using System;

namespace XiheFramework.Core.Serialization {
    [Serializable]
    public abstract class SaveData {
        public string name;
        public string time;
        public string note;
    }
}