## Chimera.UI.ComponentModel

Provides key components for building XAML-based UI using MVVM pattern.

[![NuGet package](https://img.shields.io/nuget/v/Chimera.UI.ComponentModel.svg?style=flat-square)](https://www.nuget.org/packages/Chimera.UI.ComponentModel)

The package contains two key components for building XAML-based UI using [MVVM](https://msdn.microsoft.com/en-us/library/hh848246.aspx) pattern, which supports interaction with UI through specified [SynchronizationContext](https://docs.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext?view=netstandard-1.1) and can be used in a .NET Standard assembly:

## A bindable object

A base object for view-model components, which supports two types of them:

### Type 1

```csharp
class MyViewModel : BindableObject
{
    private int _value;

    public int Value
    {
        get => GetValue(ref _value);
        set => SetValue(ref _value, value);
    }
}
```

### Type 2

```csharp
class MyViewModel : BindableObject
{
    private MyBusinessObject _object;

    public int Value
    {
        get => GetValue(_object, nameof(_object.Value), 0);
        set => SetValue(_object, nameof(_object.Value), value);
    }
}
```

## A bindable command

An extensible object for a command

```csharp
var command = new BindableCommand(MyCommandAction, MyCommandPredicate);
```

```csharp
var command = new BindableCommand(MyCommandAction, MyCommandPredicate, SynchronizationContext.Current);
```

```csharp
var command = new BindableCommand(this, MyCommandAction, MyCommandPredicate);

command.StartTrackingProperty(nameof(Value));
command.StopTrackingProperty(nameof(Value));
```