using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework
{
    public class LocalizationSvc
    {
        public static string GetValue(string key)
        {
            return Game.Localization.GetValue(key);
        }
    }
}

