using System;
using XiheFramework.Modules.Base;

namespace XiheFramework.Services {
    public static class FsmSvc {
        public static string GetState(string fsmName) {
            return Game.Fsm.GetCurrentState(fsmName);
        }

        public static void RegisterCallbacks(string fsmName, string stateName, Action onStartCallback, Action onUpdateCallback,
            Action onExitCallback, bool defaultState) {
            Game.Fsm.AddFlowState(fsmName, stateName, onStartCallback, onUpdateCallback, onExitCallback);

            if (defaultState) Game.Fsm.SetDefaultState(fsmName, stateName);
        }

        public static void SetDefaultState(string fsmName, string stateName) {
            Game.Fsm.SetDefaultState(fsmName, stateName);
        }

        public static void ChangeState(string fsmName, string stateName) {
            Game.Fsm.ChangeState(fsmName, stateName);
        }
    }
}