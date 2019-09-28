# Anemonis.UI.ComponentModel

A set of high-performance and memory-efficient basic components for building XAML-based UI using `model-view-viewmodel` and `publish–subscribe` patterns.

| [![](https://img.shields.io/gitter/room/nwjs/nw.js.svg?style=flat-square)](https://gitter.im/anemonis/ui-component-model) | Release | Current |
|---|---|---|
| Artifacts | [![](https://img.shields.io/nuget/vpre/Anemonis.UI.ComponentModel.svg?style=flat-square)](https://www.nuget.org/packages/Anemonis.UI.ComponentModel) | [![](https://img.shields.io/myget/alexanderkozlenko/vpre/Anemonis.UI.ComponentModel.svg?label=myget&style=flat-square)](https://www.myget.org/feed/alexanderkozlenko/package/nuget/Anemonis.UI.ComponentModel) |
| Code Health | | [![](https://img.shields.io/sonar/coverage/ui-component-model?format=long&server=https%3A%2F%2Fsonarcloud.io&style=flat-square)](https://sonarcloud.io/component_measures?id=ui-component-model&metric=coverage&view=list) [![](https://img.shields.io/sonar/violations/ui-component-model?format=long&server=https%3A%2F%2Fsonarcloud.io&style=flat-square)](https://sonarcloud.io/project/issues?id=ui-component-model&resolved=false) |
| Build Status | | [![](https://img.shields.io/azure-devops/build/alexanderkozlenko/github-pipelines/10?label=master&style=flat-square)](https://dev.azure.com/alexanderkozlenko/github-pipelines/_build?definitionId=10&_a=summary) |

## Project Details

| Component | Purpose |
| --- | --- |
| `BindableObject` | A minimal implementation of the `INotifyPropertyChanged` interface |
| `ObservableObject` | An extended version of the `BindableObject` type |
| `BindableCommand<T>` | A minimal implementation of the `ICommand` interface |
| `ObservableCommand<T>` | An extended version of the `BindableCommand<T>` type |
| `DataEventBroker` | A minimal implementation of the `publish–subscribe` message bus |

## Project Details: Bindable Object

- The component supports working with a synchronization context.
- The `GetValue` method uses the specified default value if the target object is `null`.
- The `SetValue` method does nothing if the target object is `null`.
- The `SetValue` method can invoke an optional callback if the value has been changed.

## Project Details: Observable Object

- The component supports publishing events to an observer.

## Project Details: Bindable Command

- The component supports working with a synchronization context.
- The component supports strongly-typed command parameters.

## Project Details: Observable Command

- The component supports publishing events to an observer.
- The component supports automatic state update based on events from bindable objects.

## Project Details: Event Broker

- Event subscription is based on channel name and data type.

## Code Examples: Observable Object

```cs
public class EntityViewModel : ObservableObject
{
    private int _value;
    private EntityModel _entity;

    private void OnValueUpdated()
    {
    }

    public int Value1
    {
        get => GetValue(ref _value);
        set => SetValue(ref _value, value);
    }

    public int Value2
    {
        get => GetValue(ref _value, nameof(OnValueUpdated));
        set => SetValue(ref _value, value, nameof(OnValueUpdated));
    }

    public int EntityValue1
    {
        get => GetValue(_entity, nameof(_entity.Value), 0);
        set => SetValue(_entity, nameof(_entity.Value), value);
    }

    public int EntityValue2
    {
        get => GetValue(_entity, nameof(_entity.Value), 0, nameof(OnValueUpdated));
        set => SetValue(_entity, nameof(_entity.Value), value, nameof(OnValueUpdated));
    }
}
```

## Code Examples: Observable Command

```cs
public class EntityViewModel : ObservableObject
{
    private readonly IObservableCommand _command;

    private int _value;

    public EntityViewModel()
    {
        _command = new ObservableCommand<string>(ExecuteCommand);
    }

    public override void Subscribe()
    {
        _command.Subscribe(this, nameof(Value));
    }

    private void ExecuteCommand(string parameter)
    {
    }

    public int Value
    {
        get => GetValue(ref _value);
        set => SetValue(ref _value, value);
    }
}
```

## Code Examples: Event Broker

```cs
public class EntityViewModel : ObservableObject
{
    private readonly IDataEventBroker _events;

    public EntityViewModel(IDataEventBroker events)
    {
        _events = events;
    }

    public override void Subscribe()
    {
        _events.Subscribe("pipe-1", OnDataEvent);
    }

    public override void Unsubscribe()
    {
        _events.Unsubscribe("pipe-1", OnDataEvent);
    }

    private void OnDataEvent(DataEventArgs<double> args)
    {
        _events.Publish("pipe-2", $"Channel: {args.ChannelName}, Value: {args.Value}");
    }
}
```

## Code Examples

- Example of GUI application for .NET Core: https://github.com/alexanderkozlenko/avalonia-puzzle-15

## Quicklinks

- [Contributing Guidelines](./CONTRIBUTING.md)
- [Code of Conduct](./CODE_OF_CONDUCT.md)
