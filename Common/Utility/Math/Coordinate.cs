using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    [Serializable]
    public class Coordinate {
        public int x;
        public int y;
        public int z;
        public int w;

        public Coordinate(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public Coordinate(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Coordinate(int x, int y, int z, int w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
}