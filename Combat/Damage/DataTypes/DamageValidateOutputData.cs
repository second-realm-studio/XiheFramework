using UnityEngine;

namespace XiheFramework.Combat.Damage.DataTypes {
    public struct DamageValidateOutputData {
        public string senderName;
        public string[] senderBuffNames;
        public Vector3 senderWorldPosition;
        
        public string receiverName;
        public string[] receiverBuffNames;
        public Vector3 receiverWorldPosition;
    }
}