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

A base object for a platform specific command

```csharp
class MyCommand : BindableCommand, ICommand
{
}
```

which must be specified in a command factory

```csharp
BindableCommand.RegisterFactory(() => new MyCommand());
```

and then will be implicitly used by a factory method

```csharp
var command = BindableCommand.Create(MyCommandAction, MyCommandPredicate);
```