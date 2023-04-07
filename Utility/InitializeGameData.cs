using NodeCanvas.Framework;
using UnityEngine;
using XiheFramework.Modules.Base;

namespace XiheFramework.Utility {
    [RequireComponent(typeof(Blackboard))]
    public class InitializeGameData : MonoBehaviour {
        public Blackboard blackboard;

        // Start is called before the first frame update
        private void Start() {
            var iterator = blackboard.GetVariables();
            foreach (var variable in iterator) Game.Blackboard.SetData(variable.name, variable.value);
        }
    }
}