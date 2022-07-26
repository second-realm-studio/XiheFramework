using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

namespace XiheFramework {
    public abstract class HitEntity : MonoBehaviour {
        public int id = -1;
        public float damage;

        public abstract void Play();
        
        private void Start() {
            id = Game.Hit.NextId;
        }

        private void Update() {
            // if (Input.GetKeyDown(KeyCode.P)) {
            //     Play();
            // }
        }
    }
}