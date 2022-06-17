using SimpleLayer.Objects;

namespace SimpleLayer.GameEngine.Managers;

public class Broker
{
    private Stack<Event> _events;

    public Broker()
    {
        _events = new Stack<Event>();
    }

    public void AddNewEvent(Event userEvent)
    {
        _events.Push(userEvent);
    }

    public Event RemoveEvent()
    {
        return _events.Pop();
    }
}
    