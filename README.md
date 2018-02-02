## Chimera.UI.ComponentModel

A set of key components for building XAML-based UI using `MVVM` pattern, which supports interaction with UI through specified `SynchronizationContext` and can be used in a .NET Standard assembly. Each component has a corresponded interface as an additional abstraction layer and implements the `IDisposable` interface. The package has no additional dependencies and can be used for creating a platform-independent testable library of bindable components.

[![NuGet package](https://img.shields.io/nuget/v/Chimera.UI.ComponentModel.svg?style=flat-square)](https://www.nuget.org/packages/Chimera.UI.ComponentModel)

### `BindableObject`

A base class for bindable components, which supports two types of bindable object structures. `SetValue` has an optional `Action` parameter to specify an action, which will be executed in case the value was changed during `SetValue` method invocation.

#### Bindable Object: Type 1

A bindable component, which works with values stored directly inside the component.

```cs
public class MyBindableObject1 : BindableObject
{
    private int _value;

    public int Value
    {
        get => GetValue(ref _value);
        set => SetValue(ref _value, value);
    }
}
```

#### Bindable Object: Type 2

A bindable component, which works with values from existing business object.

- `GetValue` method uses the specified default value to return in case the target business object is `null`.
- `SetValue` method does nothing in case the target business object is `null`.

```cs
public class MyBindableObject2 : BindableObject
{
    private MyBusinessObject _target;

    public int Value
    {
        get => GetValue(_target, nameof(_target.Value), 0);
        set => SetValue(_target, nameof(_target.Value), value);
    }
}
```

### `BindableCommand`

An extensible object for a bindable command, which supports tracking the `PropertyChanged` event for the specified object and provided property names.

```cs
var command = new BindableCommand(CommandAction, CommandPredicate);
```
or
```cs
var command = new BindableCommand(CommandAction, CommandPredicate, this);

command.StartTrackingProperty(nameof(Value));
command.StopTrackingProperty(nameof(Value));
```