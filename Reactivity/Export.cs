using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Reactivity;

using EffectAction = OnStateChange;

/// <summary>
/// The delegate returned from <see cref="React.UseEffect(OnStateChange, DependencyList)"/>.
/// </summary>
public delegate void RemoveEffect();

/// <summary>
/// A global hook. This delegate will be called whenever ANY state changes.
/// </summary>
/// <param name="stateful"></param>
public delegate void GlobalHook(dynamic stateful);

/// <summary>
/// The reactive export class. All functions you need live here.
/// </summary>
public static class React
{
    private static readonly List<GlobalHook> _globalHooks = [];

    /// <summary>
    /// Initializes a new stateful object, encapsulating <paramref name="default"/>.
    /// To access the value after this, you will call <see cref="Value{T}(Stateful{T})"/>.
    /// To set the state, use the second return value.
    /// </summary>
    /// <example>
    /// var (state, setState) = React.UseState(69);
    /// 
    /// setState(70);
    /// </example>
    /// <typeparam name="T"></typeparam>
    /// <param name="default">The initial value.</param>
    /// <returns>The stateful, and a function to mutate the stateful.</returns>
    public static KeyValuePair<Stateful<T>, Action<T>> UseState<T>(T @default)
    {
        var state = new Stateful<T>(@default);
        return new KeyValuePair<Stateful<T>, Action<T>>(state, state.SetState);
    }

    /// <summary>
    /// Create an effect that will occur when any of the (<paramref name="dependencies"/>) change.
    /// </summary>
    /// <param name="effect">The effect that will occur.</param>
    /// <param name="dependencies">All dependencies, when any of these change, <paramref name="effect"/> will be called.</param>
    /// <returns>A function to remove this effect from all of the <paramref name="dependencies"/>.</returns>
    public static RemoveEffect UseEffect(EffectAction effect, DependencyList dependencies)
    {
        foreach (dynamic dep in dependencies.Dependencies)
        {
            dep.OnChange(effect);
        }

        return () =>
        {
            foreach (dynamic dep in dependencies.Dependencies)
            {
                dep.Remove(effect);
            }
        };
    }

    /// <summary>
    /// This provides a way to have state that is excempt from the Reactive system.
    /// Any subsequent calls to this function will return the same reference to-value, as 
    /// it's stored globally.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="default">The default value.</param>
    /// <param name="line">Automatically filled, do not manually provide.</param>
    /// <param name="fp">Automatically filled, do not manually provide.</param>
    /// <returns>The <see cref="Ref{T}"/> object, which will be the same as the last time you called it.</returns>
    public static Ref<T> UseRef<T>(T @default, [CallerLineNumber] int line = 0, [CallerFilePath] string fp = "")
    {
        var hash = $"{fp}:{line}".GetHashCode();

        // This means each time the ref is created (when the function is called)
        // It will get the initial one that was created.
        if (GlobalState.Refs.TryGetValue(hash, out RefRef? value))
            return value.Ref;

        var @ref = new Ref<T>(@default);

        GlobalState.Refs.Add(hash, new RefRef() { Ref = @ref });

        return @ref;
    }

    /// <summary>
    /// Have an action occur When something happens. See <see cref="It{T}(Stateful{T})"/> for dealing with
    /// the first argument.
    /// </summary>
    /// <example>
    /// React.When(React.It(state).MeetsCondition(...), (state) => ...);
    /// </example>
    /// <typeparam name="T"></typeparam>
    /// <param name="state">The state, typically build via <see cref="React.It{T}(Stateful{T})"/></param>
    /// <param name="action">The action, called when <paramref name="state"/> meets the conditions.</param>
    public static void When<T>(WhenState<T> state, Action<T> action)
    {
        UseEffect(() =>
        {
            bool computed = false;

            for (var i = 0; i < state.InOrderComputeActions.Count; i++) 
            {
                computed |= state.InOrderComputeActions[i]();
            }

            if (computed)
            {
                action(Value(state.Stateful));
            }

        }, new DependencyList([state.Stateful]));
    }

    /// <summary>
    /// Wrap a <see cref="Stateful{T}"/> object into something that can store and deal with
    /// computations around the <see cref="Stateful{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stateful">The stateful itself, created with <see cref="React.UseState{T}(T)"/></param>
    /// <returns></returns>
    public static WhenState<T> It<T>(Stateful<T> stateful)
    {
        return new WhenState<T>(stateful);
    }

    /// <summary>
    /// See if there's installed global hooks.
    /// </summary>
    /// <returns></returns>
    public static bool HasGlobalHook() =>  _globalHooks.Count > 0;

    /// <summary>
    /// Get all installed global hooks.
    /// </summary>
    /// <returns></returns>
    public static List<GlobalHook> GlobalHooks() => _globalHooks;

    /// <summary>
    /// Install a global hook.
    /// </summary>
    /// <param name="hook"></param>
    public static void InstallGlobalHook(GlobalHook hook)
    {
        _globalHooks.Add(hook); 
    }

    /// <summary>
    /// Enable verbose output. Useful for debugging. This installes a global hook,
    /// and gives pretty output regarding changes.
    /// </summary>
    /// <param name="enabled"></param>
    public static void EnableVerboseDebugOutputting(bool enabled)
    {
        static void debugHook(dynamic value)
        {
            Type type = value.GetType();
            Console.WriteLine($"[debug] change  :  {value}  :  {type.Module.Name}  :  type({type.Name})");
        }

        if (!enabled)
        {
            _globalHooks.Remove(debugHook);
        }
        else
        {
            Console.WriteLine(" \\- Verbose Debug Output - Enabled /-");
            _globalHooks.Add(debugHook);
        }
    }

    /// <summary>
    /// Unwrap the value of a <see cref="Stateful{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stateful"></param>
    /// <returns></returns>
    public static T Value<T>(Stateful<T> stateful)
    {
        return stateful.Unwrap();
    }
}
