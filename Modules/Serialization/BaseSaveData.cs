using System.Collections.Generic;

namespace XiheFramework.Modules.Serialization {
    public class BaseSaveData {
        public string name; //save file name

        public Dictionary<string, object> saveData;
        public string time; //real time when the file is written
    }
}