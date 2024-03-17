using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework.Core.Resource {
    [CreateAssetMenu(fileName = "LevelDataSetting", menuName = "Xihe/Resource/LevelDataSetting")]
    public class LevelDataSetting : ScriptableObject
    {
        public List<string> dataLabels;
        // public List<AssetReference> dataReferences;
    }
}
