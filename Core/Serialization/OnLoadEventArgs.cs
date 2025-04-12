using System;
using XiheFramework.Core.Serialization.Serializable;

namespace XiheFramework.Core.Serialization {
    public struct OnLoadEventArgs {
        public DateTime timeStamp;
        public SerializableEntityData entityData;
    }
}