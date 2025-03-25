
namespace Reactivity;

public interface IReadonlyProperty<T>
{
    /// <summary>
    /// Provides a getter, which will (under the hood)
    /// call <see cref="React.Value{T}(Stateful{T})"/>. This is the
    /// readonly variant, so it cannot be set.
    /// </summary>
    public T Value { get; }
}

/// <summary>
/// A class property that is reactive and stateful.
/// </summary>
public class Property<T> : IReadonlyProperty<T>
{
    private readonly Stateful<T> _stateful;
    private readonly Action<T> _setStateful;

    public Property(T value)
    {
        (_stateful, _setStateful) = React.UseState(value);
    }

    /// <summary>
    /// Get the state, for use in UseEffect's etc.
    /// </summary>
    /// <returns></returns>
    public Stateful<T> State() => _stateful;

    /// <summary>
    /// Provides a getter and setter, which will (under the hood)
    /// call <see cref="React.Value{T}(Stateful{T})"/> and <c>setState</c>, causing this
    /// property to apply effects, etc.
    /// </summary>
    public T Value
    {
        get
        {
            return React.Value(_stateful);
        }

        set
        {
            _setStateful(value);
        }
    }
}
