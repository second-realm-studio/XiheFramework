using System.Reflection;
using ParadoxNotion;

namespace FlowCanvas.Nodes
{
    //Encapsulation of EventInfo. This acts similar to how UnityEventBase does for Unity
    abstract public class SharpEvent
    {

        public object instance { get; private set; }
        public EventInfo eventInfo { get; private set; }

        ///<summary>Create SharpEvent<T> wrapper for target EventInfo</summary>
        public static SharpEvent Create(EventInfo eventInfo) {
            if ( eventInfo == null ) { return null; }
            var wrapper = (SharpEvent)typeof(SharpEvent<>).RTMakeGenericType(eventInfo.EventHandlerType).CreateObjectUninitialized();
            wrapper.eventInfo = eventInfo;
            return wrapper;
        }

        ///<summary>Set target instance of event</summary>
        public void SetInstance(object instance) {
            this.instance = instance;
        }

        ///<summary>Start listening to a reflected delegate event using this wrapper</summary>
        public void StartListening(ReflectedDelegateEvent reflectedEvent, ReflectedDelegateEvent.DelegateEventCallback callback) {
            if ( reflectedEvent == null || callback == null ) { return; }
            reflectedEvent.Add(callback);
            eventInfo.AddEventHandler(instance, reflectedEvent.AsDelegate());
        }

        ///<summary>Stop listening from a reflected delegate event using this wrapper</summary>
        public void StopListening(ReflectedDelegateEvent reflectedEvent, ReflectedDelegateEvent.DelegateEventCallback callback) {
            if ( reflectedEvent == null || callback == null ) { return; }
            reflectedEvent.Remove(callback);
            eventInfo.RemoveEventHandler(instance, reflectedEvent.AsDelegate());
        }
    }

    ///<summary>Typeof(T) is the event handler type</summary>
    public class SharpEvent<T> : SharpEvent
    {

    }

}