using System;
using System.ComponentModel;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

internal class NotifyPropertyChangedAttribute : TypeAspect
{
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
                setTemplate: nameof(this.OverridePropertySetter));
        }
    }

    [InterfaceMember]
    public event PropertyChangedEventHandler? PropertyChanged;

    [Introduce]
    protected void OnPropertyChanged(string name)
    {
        this.PropertyChanged?.Invoke(meta.This, new PropertyChangedEventArgs(name));
    }

    [Template]
    private dynamic OverridePropertySetter(dynamic value)
    {
        var oldValue = meta.Target.Property.Value;

        if (!Equals(oldValue, value))
        {
            meta.Proceed();
            this.OnPropertyChanged(meta.Target.Property.Name);

            Console.WriteLine(
                $"{meta.Target.Property.Name} has been updated to '{value}' and the old value was '{oldValue}'");
        }

        return value;
    }
}

