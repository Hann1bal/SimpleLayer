using SimpleLayer.Objects;

namespace SimpleLayer.GameEngine.Managers;

public class Broker
{
    public Stack<Event> Events;

    public Broker(Stack<Event> events)
    {
        Events = events;
    }

    public void AddNewEvent(Event userEvent)
    {
        Events.Push(userEvent);
    }

    public Event RemoveEvent()
    {
        return Events.Pop();
    }
}
    