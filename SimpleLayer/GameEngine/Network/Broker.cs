using SimpleLayer.Objects;

namespace SimpleLayer.GameEngine.Managers;

public class Broker
{
    public Stack<Event> Events;
    public Stack<Event> ReceiveEvents;

    public Broker(Stack<Event> events, Stack<Event> receiveEvents)
    {
        Events = events;
        ReceiveEvents = receiveEvents;
    }

    public void AddNewEvent(Event userEvent)
    {
        Events.Push(userEvent);
    }

    public Event GetEvent()
    {
        return ReceiveEvents.Pop();
    }
}