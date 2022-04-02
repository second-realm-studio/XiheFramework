using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace XiheFramework
{
    public class SerializationModule : GameModule
    {
        private string path = Application.streamingAssetsPath + "/";
        private const string suffix = ".saori";
        public void WriteSaveData(string fileName)
        {
            //序列化过程（将save转化为字节流）
            //创建Save对象并保存当前游戏状态
            BaseSaveData baseSaveData = Game.Blackboard.CreateSaveData();
            //创建一个二进制格式化程序
            BinaryFormatter bf = new BinaryFormatter();
            //创建一个文件流
            FileStream fileStream = File.Create(path + fileName + suffix);
            //用二进制格式化程序的序列化方法来序列化Save对象,参数：创建的文件流和需要序列化的对象
            bf.Serialize(fileStream, baseSaveData);
            //关闭流
            fileStream.Close();

            /*if (File.Exists(Application.dataPath + "/StreamingFile" + "/byBin.txt"))
            {
            //若保存文件存在则执行
            
            }*/
        }

        public void ReadSaveData(string fileName)
        {
            if (!File.Exists(path + fileName + suffix))
            {
                return;
            }

            //反序列化过程
            //创建一个二进制格式化程序
            BinaryFormatter bf = new BinaryFormatter();
            //打开一个文件流
            FileStream fileStream =
                File.Open(path + fileName + suffix, FileMode.Open);
            //调用格式化程序的反序列化方法，将文件流转化为一个Save对象
            BaseSaveData saoriSave = (BaseSaveData) bf.Deserialize(fileStream);
            //关闭文件流
            fileStream.Close();
            //向Blackboard传输save中的数据
            Game.Blackboard.LoadSaveData(saoriSave);
        }
        
        public bool IfRecoverSaveFile(string fileName)
        {
            //m_ReadyToSaveFileName = SaveName;
            if (File.Exists(path + fileName + suffix))
            {
                return true;
            }
            else
            {
                WriteSaveData(fileName);
                return false;
            }
        }

        public bool IfLoadFileEmpty(string fileName)
        {
            if (File.Exists(path + fileName + suffix))
            {
                return true;
            }

            return false;
        }

        public string GetSaveInfo(string fileName)
        {
            if (File.Exists(path + fileName + suffix))
            {
                //反序列化过程
                //创建一个二进制格式化程序
                BinaryFormatter bf = new BinaryFormatter();
                //打开一个文件流
                FileStream fileStream =
                    File.Open(path + fileName + suffix, FileMode.Open);
                //调用格式化程序的反序列化方法，将文件流转化为一个Save对象
                BaseSaveData baseSaveData = (BaseSaveData) bf.Deserialize(fileStream);
                //关闭文件流
                fileStream.Close();
                return baseSaveData.name + "/n" + baseSaveData.time;
            }

            return null;
        }

        public override void Update()
        {
        }

        public override void ShutDown(ShutDownType shutDownType)
        {
        }
    }
}