using SimpleLayer.GameEngine.Network.EventModels;
using SimpleLayer.GameEngine.Network.Types;

namespace SimpleLayer.GameEngine.Network;

public class Broker
{
    public Stack<BuildingEvent> Events;
    public Stack<BuildingEvent> ReceiveEvents;
    public EventType Type;

    public Broker(Stack<BuildingEvent> events, Stack<BuildingEvent> receiveEvents)
    {
        Events = events;
        ReceiveEvents = receiveEvents;
    }

    public void AddNewEvent(BuildingEvent userEvent)
    {
        Events.Push(userEvent);
    }

    public BuildingEvent GetEvent()
    {
        return ReceiveEvents.Pop();
    }
}