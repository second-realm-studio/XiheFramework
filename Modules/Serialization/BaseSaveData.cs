using System.Collections.Generic;

namespace XiheFramework {
    public class BaseSaveData {
        public string name;//save file name
        public string time;//real time when the file is written

        public Dictionary<string, object> saveData;
    }
}