## Anemonis.UI.ComponentModel

A set of basic components for building XAML-based UI using `model-view-viewmodel` pattern.

[![NuGet package](https://img.shields.io/nuget/v/Anemonis.UI.ComponentModel.svg?style=flat-square)](https://www.nuget.org/packages/Anemonis.UI.ComponentModel)

### Important Features

- The bindable object as a simple base type for view models.
- The bindable command as a simple and extensible command implementation.
- The event broker as a simple messaging bus for UI events based on the `publish–subscribe` pattern.

### Important Features: Bindable Object

- The component supports working with a synchronization context.
- The `GetValue` method uses the specified default value if the target object is `null`.
- The `SetValue` method does nothing if the target object is `null`.
- The `SetValue` method can invoke an optional callback if the value was changed.

### Important Features: Bindable Command

- The component supports working with a synchronization context.
- The component supports automatic state refresh based on properties update.

### Important Features: Event Broker

- Event subscription is based on channel name and data type.

### Usage Examples: Bindable Object

```cs
public class MyBindableObject : BindableObject
{
    private int _value;

    private void OnPropertyUpdate()
    {
    }

    public int Value1
    {
        get => GetValue(ref _value);
        set => SetValue(ref _value, value);
    }

    public int Value2
    {
        get => GetValue(ref _value, nameof(OnPropertyUpdate));
        set => SetValue(ref _value, value, nameof(OnPropertyUpdate));
    }
}
```
```cs
public class MyBindableObject : BindableObject
{
    private MyTargetObject _target;

    private void OnPropertyUpdate()
    {
    }

    public int Value1
    {
        get => GetValue(_target, nameof(_target.Value), 0);
        set => SetValue(_target, nameof(_target.Value), value);
    }

    public int Value2
    {
        get => GetValue(_target, nameof(_target.Value), 0, nameof(OnPropertyUpdate));
        set => SetValue(_target, nameof(_target.Value), value, nameof(OnPropertyUpdate));
    }
}
```

### Usage Examples: Bindable Command

```cs
public class MyBindableObject : BindableObject
{
    private readonly IBindableCommand _command;

    private int _value;

    public MyBindableObject()
    {
        _command = new BindableCommand<string>(CommandAction);
        _command.SubscribePropertyChanged(this, nameof(Value));
    }

    private void CommandAction(string parameter)
    {
    }

    public int Value
    {
        get => GetValue(ref _value);
        set => SetValue(ref _value, value);
    }
}
```

### Usage Examples: Event Broker

```cs
public class MyBindableObject : BindableObject
{
    private readonly IDataEventBroker _events;

    public MyBindableObject(IDataEventBroker events)
    {
        _events = events;
    }

    private void SubscribeToEvents()
    {
        _events.Subscribe("channel-1", OnChannelEvent);
    }

    private void UnsubscribeFromEvents()
    {
        _events.Unsubscribe("channel-1", OnChannelEvent);
    }

    private void OnChannelEvent(double value)
    {
        _events.Publish("channel-2", "value: " + value);
    }
}
```

### Usage Examples

- Example of GUI application for .NET Core: https://github.com/alexanderkozlenko/avalonia-puzzle-15