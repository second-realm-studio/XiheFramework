using System;

namespace XiheFramework.Modules.Blackboard {
    [Serializable]
    public struct BlackBoardObject {
        public BlackBoardDataType type;
        public object entity;

        public BlackBoardObject(object entity, BlackBoardDataType type) {
            this.entity = entity;
            this.type = type;
        }
    }
}