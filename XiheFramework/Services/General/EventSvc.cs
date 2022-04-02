namespace XiheFramework {
    public static class EventSvc {
        public static void Invoke(string eventName, object sender, object value) {
            //GameManager.GetModule<EventManager>().Invoke(eventName, value);
            Game.Event.Invoke(eventName, sender, value);
        }
    }
}