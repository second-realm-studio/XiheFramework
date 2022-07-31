using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

namespace XiheFramework {
    public abstract class HitEntity : MonoBehaviour {
        public string guid;
        public float damage; //standard damage 
        public float duration = 1f;
        public float coolDown = 0.4f; //define the cooldown time for a hit entity to be able to hit a same target again

        //public Vector3 damageDirection;

        public abstract void Play();

        protected virtual void Start() {
            RefreshGuid();
        }

        protected void RefreshGuid() {
            guid = Game.Hit.GetNewGuid();
        }
    }
}