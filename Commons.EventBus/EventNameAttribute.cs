namespace Commons.EventBus;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EventNameAttribute : Attribute
{
    public string Name { get; init; }
    public EventNameAttribute(string name)
    {
        Name = name;
    }

}
