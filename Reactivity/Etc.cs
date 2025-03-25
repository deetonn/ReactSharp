
namespace Reactivity;

public class IdOptions
{
    /// <summary>
    /// The smallest (inclusive) the ID can be.
    /// </summary>
    public int Minimum { get; set; } = 0;

    /// <summary>
    /// The largest (inclusive) the ID can be.
    /// </summary>
    public int Maximum { get; set; } = int.MaxValue;
}

/// <summary>
/// For more niche hooks and features that aren't all too useful.
/// </summary>
internal class Etc
{
    internal static Dictionary<int, int> Ids = [];
    internal static Dictionary<int, object> Cache = [];

    public static int GenerateUniqueId(string callerLocation, int callerLine, IdOptions? options = null)
    {
        var hashCode = $"{callerLocation}:{callerLine}-useId".GetHashCode();
        
        if (Ids.TryGetValue(hashCode, out var existingId))
            return existingId;

        int uniqueId;
        if (options is not null)
        {
            uniqueId = Random.Shared.Next(options.Minimum, options.Maximum);
        }
        else
        {
            uniqueId = Random.Shared.Next();
        }

        Ids.Add(hashCode, uniqueId);

        return uniqueId;
    }
}
