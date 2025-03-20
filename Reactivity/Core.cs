
namespace Reactivity;

/// <summary>
/// The state of a "When" call. This encapsulates the <see cref="Stateful{T}"/> that
/// is being watched, aswell as the compute actions.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="item"></param>
public class WhenState<T>(Stateful<T> item)
{
    /// <summary>
    /// The thing being watched.
    /// </summary>
    public Stateful<T> Stateful { get; set; } = item;

    /// <summary>
    /// Compute actions that are in order. These are executed 0..onwards.
    /// </summary>
    public List<Func<bool>> InOrderComputeActions { get; set; } = [];

    /// <summary>
    /// The callback will execute When <paramref name="value"/> Equals <see cref="Stateful{T}"/>'s value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>The same instance, to stack different things.</returns>
    public WhenState<T> Equals(T value)
    {
        InOrderComputeActions.Add(() =>  React.Value(Stateful)?.Equals(value) ?? false);
        return this;
    }

    /// <summary>
    /// The callback will execute when <paramref name="condition"/> returns <c>true</c>.
    /// </summary>
    /// <param name="condition">The custom comparer.</param>
    /// <returns>The same instance, to stack different things.</returns>
    public WhenState<T> MeetsCondition(Func<T, bool> condition)
    {
        InOrderComputeActions.Add(() =>
        {
            return condition(React.Value(Stateful));
        });
        return this;
    }
}

/// <summary>
/// A delegate that represents state changes.
/// </summary>
public delegate void OnStateChange();

/// <summary>
/// The core component. This encapsulates a value and state watchers. 
/// When the value is changed, the state watchers are called. 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="value"></param>
public class Stateful<T>(T value)
{
    private T _value = value;
    private readonly List<OnStateChange> _stateWatchers = [];

    /// <summary>
    /// Add an onchange handler. This handler will be called anytime this value
    /// is modified.
    /// </summary>
    /// <param name="stateWatcher"></param>
    public void OnChange(OnStateChange stateWatcher)
    {
        _stateWatchers.Add(stateWatcher);
    }

    /// <summary>
    /// The only way to set the state. This function is directly returned
    /// from <see cref="React.UseState{T}(T)"/> as the second return value.
    /// </summary>
    /// <param name="value"></param>
    public void SetState(T value)
    {
        _value = value;
        for (var i = 0; i < _stateWatchers.Count; i++)
        {
            _stateWatchers[i].Invoke();
        }

        if (React.HasGlobalHook())
        {
            var globalHooks = React.GlobalHooks();
            foreach (var hook in globalHooks)
            {
                hook(value!);
            }
        }
    }

    /// <summary>
    /// Remove a state watcher.
    /// </summary>
    /// <param name="stateWatcher"></param>
    public void Remove(OnStateChange stateWatcher)
    {
        _stateWatchers.Remove(stateWatcher);
    }

    /// <summary>
    /// Unwrap the value.
    /// </summary>
    /// <returns></returns>
    internal T Unwrap() => _value;

    public override string? ToString()
    {
        return _value?.ToString();
    }
}
