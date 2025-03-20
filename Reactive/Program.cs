using System.Diagnostics;
using Reactivity;

var stopwatch = new Stopwatch();
stopwatch.Start();

var (count, setCount) = React.UseState(40);
var (loading, setLoading) = React.UseState(true);

var endCondition = React.It(count).MeetsCondition((count) =>
{
    return count >= 50;
});

React.When(endCondition, (_) =>  setLoading(false));

React.UseEffect(() =>
{
    var initialCount = React.UseRef(React.Value(count));
    var diff = React.Value(count) - initialCount.Value;
    Console.WriteLine($"Current count: {React.Value(count)} ({diff}) -- Initial: {initialCount}");
}, new DependencyList([count]));

while (React.Value(loading))
{
    setCount(React.Value(count) + 1);
}

stopwatch.Stop();

Console.WriteLine($"\n\nTook {stopwatch.ElapsedMilliseconds}ms");
