## Anemonis.UI.ComponentModel

A set of key components for building XAML-based UI using Model-View-ViewModel pattern, which supports interaction with UI through specified synchronization context.

[![NuGet package](https://img.shields.io/nuget/v/Anemonis.UI.ComponentModel.svg?style=flat-square)](https://www.nuget.org/packages/Anemonis.UI.ComponentModel)

### Characteristics
### Characteristics: Bindable Object

A base class for bindable components, which supports two types of bindable object structures. `SetValue` has an optional `Action` parameter to specify an action, which will be executed in case the value was changed during `SetValue` method invocation.

### Characteristics: Bindable Object (Type 1)

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

### Characteristics: Bindable Object (Type 2)

A bindable component, which works with values from specified object.

- `GetValue` method uses the specified default value to return in case the specified object is `null`.
- `SetValue` method does nothing in case the specified object is `null`.

```cs
public class MyBindableObject2 : BindableObject
{
    private MyTargetObject _target;

    public int Value
    {
        get => GetValue(_target, nameof(_target.Value), 0);
        set => SetValue(_target, nameof(_target.Value), value);
    }
}
```

### Characteristics: Bindable Command

An extensible object for a bindable command, which supports observing the `PropertyChanged` event for the specified object and provided property names.

```cs
var command = new BindableCommand(CommandAction, CommandPredicate);
```
```cs
var command = new BindableCommand(CommandAction, CommandPredicate, this);

command.StartObservingProperties(nameof(Value));
command.StopObservingProperties(nameof(Value));
```

### Usage Examples

- Example of GUI application for .NET Core: https://github.com/alexanderkozlenko/avalonia-puzzle-15