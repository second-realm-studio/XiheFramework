using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using XiheFramework.Modules.Base;

namespace XiheFramework.Modules.Localization {
    public class LocalizationModule : GameModule {
        public Languages language = Languages.English;

        public string streamingPath = "Localization.json";

        private readonly Dictionary<string, string[]> m_LocalizationDictionary = new();

        private void Start() {
            //ReadLocalizationFile();
            //Debug.LogInfo(GetValue("上午"));
        }
        
        public string GetValue(string key) {
            return m_LocalizationDictionary[key][(int)language];
        }

        private void ReadLocalizationFile() {
            var strPath = Application.streamingAssetsPath + "/" + streamingPath;
            var fsInfo = new FileStream(strPath, FileMode.Open, FileAccess.Read);
            var srInfo = new StreamReader(fsInfo, Encoding.GetEncoding("UTF-8"));
            srInfo.BaseStream.Seek(0, SeekOrigin.Begin);
            var strResult = srInfo.ReadToEnd();

            var jObject = (JObject)JsonConvert.DeserializeObject(strResult);
            foreach (var i in jObject) {
                string[] strArray = { jObject[i.Key]["eng"].ToString(), jObject[i.Key]["jap"].ToString() };
                m_LocalizationDictionary.Add(i.Key, strArray);
            }
        }

        internal override void ShutDown(ShutDownType shutDownType) { }
    }
}