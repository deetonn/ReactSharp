
namespace Reactivity;

/// <summary>
/// A list of values that are dependants to an effect. If any change,
/// the effect occurs.
/// </summary>
public class DependencyList
{
    private readonly List<dynamic> _dependencies = [];

    public DependencyList(dynamic[] statefuls)
    {
#if DEBUG
        /* The `statefuls[idx].GetType()` will always run slow on first run, for each different dependency.
           This is because it's being resolved, any subsequent runs after that will be quick.

           Hence why this check is disabled in release.
         */
        for (var idx = 0; idx < statefuls.Length; idx++)
        {
            Type type = statefuls[idx].GetType();

            if (!type.Name.Contains("Stateful`"))
            {
                throw new ArgumentException($"dependency #{idx + 1} is not the correct type. (expected Stateful, got {type.Name})");
            }
        }
#endif

        _dependencies = [.. statefuls];
    }

    /// <summary>
    /// The total amount of dependencies in this list.
    /// </summary>
    public int Length => _dependencies.Count;

    /// <summary>
    /// The dependencies themselves. These are dynamic due to the nature of
    /// generics. 
    /// </summary>
    public IList<dynamic> Dependencies => _dependencies;
}
