using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XiheFramework;

namespace XiheFramework {
    public static class FsmSvc {
        public static string GetState(string fsmName) {
            return Game.Fsm.GetCurrentState(fsmName);
        }
    }
}