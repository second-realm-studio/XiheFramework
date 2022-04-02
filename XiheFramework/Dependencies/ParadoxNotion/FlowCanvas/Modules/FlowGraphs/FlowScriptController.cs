using NodeCanvas.Framework;

namespace FlowCanvas
{

    ///<summary>Add this component on a game object to be controlled by a FlowScript</summary>
    [UnityEngine.AddComponentMenu("FlowCanvas/FlowScript Controller")]
    public class FlowScriptController : GraphOwner<FlowScript>
    {
        ///<summary>Calls a custom function in the flowgraph. This overload exists so that works with UnityEvents</summary>
        public void CallFunction(string name) {
            behaviour.CallFunction(name);
        }

        ///<summary>Calls and returns a value of a custom function in the flowgraph</summary>
        public object CallFunction(string name, params object[] args) {
            return behaviour.CallFunction(name, args);
        }

        ///<summary>Calls a custom function in the flowgraph async. When the function is done, it will callback with return value</summary>
        public void CallFunctionAsync(string name, System.Action<object> callback, params object[] args) {
            behaviour.CallFunctionAsync(name, callback, args);
        }
    }
}