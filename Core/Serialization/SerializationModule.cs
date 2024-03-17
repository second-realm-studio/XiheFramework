using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using XiheFramework.Core.Base;

namespace XiheFramework.Core.Serialization {
    public class SerializationModule : GameModule {
        public string suffix = ".xihe";
        private readonly string m_Path = Application.streamingAssetsPath + "/";

        private SaveData m_ActiveSaveData;

        public void WriteSaveData(string fileName) {
            var bf = new BinaryFormatter();
            var fileStream = File.Create(m_Path + fileName + suffix);
            bf.Serialize(fileStream, m_ActiveSaveData);
            fileStream.Close();
        }

        public SaveData ReadSaveData(string fileName) {
            if (!File.Exists(m_Path + fileName + suffix)) return null;

            var bf = new BinaryFormatter();
            var fileStream = File.Open(m_Path + fileName + suffix, FileMode.Open);
            var saoriSave = (SaveData)bf.Deserialize(fileStream);
            fileStream.Close();
            return saoriSave;
        }

        public bool IfRecoverSaveFile(string fileName, SaveData saveData) {
            if (File.Exists(m_Path + fileName + suffix)) {
                return true;
            }

            WriteSaveData(fileName);
            return false;
        }

        public bool IfLoadFileEmpty(string fileName) {
            return !File.Exists(m_Path + fileName + suffix);
        }

        public string GetSaveInfo(string fileName) {
            if (File.Exists(m_Path + fileName + suffix)) {
                var bf = new BinaryFormatter();
                var fileStream = File.Open(m_Path + fileName + suffix, FileMode.Open);
                var baseSaveData = (SaveData)bf.Deserialize(fileStream);
                fileStream.Close();
                return baseSaveData.name + "\n" + baseSaveData.time;
            }

            return null;
        }
    }
}