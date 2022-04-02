using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XiheFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LocalizationModule : GameModule
{
    public Languages language = Languages.English;

    public string streamingPath="Localization.json";

    private Dictionary<string, string[]> m_LocalizationDictionary = new Dictionary<string, string[]>();

    public string GetValue(string key)
    {
        return m_LocalizationDictionary[key][(int) language];
    }

    private void ReadLocalizationFile()
    {
        string strPath = Application.streamingAssetsPath+"/"+streamingPath;
        FileStream fsInfo = new FileStream(strPath, FileMode.Open, FileAccess.Read);
        StreamReader srInfo = new StreamReader(fsInfo, System.Text.Encoding.GetEncoding("UTF-8"));
        srInfo.BaseStream.Seek(0, SeekOrigin.Begin);
        string strResult = srInfo.ReadToEnd();

        JObject jObject = (JObject) JsonConvert.DeserializeObject(strResult);
        foreach (var i in jObject)
        {
            string[] strArray = new string[] {jObject[i.Key]["eng"].ToString(), jObject[i.Key]["jap"].ToString()};
            m_LocalizationDictionary.Add(i.Key,strArray);
        }
    }

    private void Start()
    {
        ReadLocalizationFile();
        //Debug.LogInfo(GetValue("上午"));
    }

    public override void Update()
    {
    }

    public override void ShutDown(ShutDownType shutDownType) {
        
    }
}