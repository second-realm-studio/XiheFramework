namespace XiheFramework {
    public static class BlackBoardSvc
    {
        public static void SetValue<T>(string key,T value, BlackBoardDataType targetType){
            Game.Blackboard.SetData<T>(key, value,targetType);
        }
    
        public static T GetValue<T>(string key)
        {
            return Game.Blackboard.GetData<T>(key);
        }
    }
}

