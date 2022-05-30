using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    [Serializable]
    public class RequestPathArgs {
        public Vector3 startPos;
        public GridBlock targetBlock;
        public bool includeDiagonalPath;

        public RequestPathArgs(Vector3 startPos, GridBlock targetBlock, bool includeDiagonalPath) {
            this.startPos = startPos;
            this.targetBlock = targetBlock;
            this.includeDiagonalPath = includeDiagonalPath;
        }
    }
}

