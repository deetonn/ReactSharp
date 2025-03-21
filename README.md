# ReactSharp 

Do you like the *React* way of doing things? Me too, so I cooked up something strange. Take advantage of the React state and effect system in C#.

### How does it work?

It works the same as React (without rendering). You create effects that occurs when dependants (state) change.

### Examples

#### Basics - Hello World

```cs
using Reactivity;

var (message, setMessage) = React.UseState(string.Empty);

React.UseEffect(() => {
	Console.WriteLine(message);
}, new DependencyList([message]));

setMessage("Hello, World!");
```

When `setMessage` is called, it triggers the effect, causing the (new) message to be written to the console.

#### State Example

```cs
using Reactivity;

var (name, setName) = React.UseState("unknown");
var (age, setAge) = React.UseState(0);

var (message, setMessage) = React.UseState(string.Empty);

React.UseEffect(() =>
{
    setMessage($"{name} is {age} years old.");
}, new DependencyList([name, age]));

setName("Deeton");
setAge(21);

Console.WriteLine(message);
```

The effect causes `message` to be populated when `name, age` are modified.

#### References
To opt-out of scope-based state, you can use `React.UseRef`. This is stored in global storage, and specific to the calling location, meaning it retains state between effect invocations.

```cs
using Reactivity;

var (count, setCount) = React.UseState(69);

React.UseEffect(() =>
{
    // Initialized on first-run, so this would be 70.
    // The way this is written, this will always be 70.
    var initialCount = React.UseRef(count);
    Console.WriteLine($"count: {count}, initial: {initialCount.Value}");
}, new DependencyList([count]));

// Causes the effect to trigger.
setCount(70);

// Causes the effect to trigger, but "initialCount" will not change.
setCount(71);
```

### Performance Notes

The `DependencyList` can be slow when type-checking. This is due to the *dynamic* type being used. When we call `GetType()` to ensure the name contains "*Stateful`*", it can take ~30ms on the first call. In release mode, this check isn't included.

If one piece of state has many `UseEffect` calls with it in the dependency list, changing the value of it may become cumbersome. This is due to the implementation. It stores callbacks in a list, on the `Stateful<T>` instance, and when you  `UseEffect`, that callback is simply added to that list. Then, when you call `setXXX`, the callbacks associated with said `Stateful<T>` are called. 

Having a singular *setState* call 50 different callbacks will obviously cause performance issues.

### Conclusion
This is pretty worthless. I just wanted to understand deeper into how React may be implemented, so I tried to implement something similar. I don't see many use-cases where this may be useful, but maybe I'll prove myself wrong in the future.
