using System;

namespace XiheFramework {
    [Serializable]
    public struct BlackBoardObject {
        public object entity;
        public BlackBoardDataType type;

        public BlackBoardObject(object entity, BlackBoardDataType type) {
            this.entity = entity;
            this.type = type;
        }
    }
}