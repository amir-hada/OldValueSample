using System.ComponentModel;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

internal class NotifyPropertyChangedAttribute : TypeAspect
{
    [Introduce]
    public event PropertyChangedEventHandler? PropertyChanged;

    [Introduce]
    protected void OnPropertyChanged(string name)
    {
        this.PropertyChanged?.Invoke(meta.This, new PropertyChangedEventArgs(name));
    }
    
    //This is the overload.
    [Introduce]
    protected void OnPropertyChanged(string name, object? oldValue, object? newValue)
    {
        this.PropertyChanged?.Invoke(meta.This, new PropertyChangedEventArgs(name));
        
        Console.WriteLine($"Property {name} changed from {oldValue} to {newValue}");
    }
    
    public override void BuildAspect(IAspectBuilder<INamedType> builder)
    {
        builder.Advice.ImplementInterface(
            builder.Target,
            typeof(INotifyPropertyChanged),
            OverrideStrategy.Ignore);

        foreach (var property in builder.Target.Properties)
        {
            builder.Advice.OverrideAccessors(
                property,
                setTemplate: nameof(OverridePropertySetter));
        }
    }

    [Template]
    private void OverridePropertySetter(dynamic value)
    {
        var oldValue = meta.Target.Property.Value;

        if (!Equals(oldValue, value))
        {
            meta.Proceed();
            
            //this.OnPropertyChanged(meta.Target.Property.Name);
            this.OnPropertyChanged(meta.Target.Property.Name, oldValue, value);

      
            Console.WriteLine(
                $"{meta.Target.Property.Name} updated to '{value}' (was '{oldValue}')");
        }
    }
    
}
