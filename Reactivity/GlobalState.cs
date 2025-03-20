
namespace Reactivity;

/// <summary>
/// A ref is global state, which lives outside the bounds of the "Reactive" layer.
/// Refs cannot cause a re-compute, they are mutable and stateless.
/// </summary>
/// <typeparam name="T"></typeparam>
public class Ref<T>(T value)
{
    private T _value = value;

    public T Value { get => _value; set => _value = value; }

    public override string? ToString()
    {
        return Value?.ToString();
    }
}

/// <summary>
/// Typeless reference storage.
/// </summary>
public class RefRef
{
    public required dynamic Ref { get; set; }
}

public static class GlobalState
{
    public static readonly Dictionary<int, RefRef> Refs = [];
}
