using System.Collections.Generic;
using ParadoxNotion;

namespace FlowCanvas
{

    ///<summary>Data struct that is propagated within the graph through the FlowPorts</summary>
    [ParadoxNotion.Design.SpoofAOT]
    public struct Flow
    {

        ///<summary>Contains data for Return calls</summary>
        public struct ReturnData
        {
            public FlowReturn returnCall { get; private set; }
            public System.Type returnType { get; private set; }
            public ReturnData(FlowReturn call, System.Type type) {
                returnCall = call;
                returnType = type;
            }
        }

        ///<summary>Number of ticks this Flow has made</summary>
        public int ticks { get; internal set; }

        private Dictionary<string, object> parameters;
        private ReturnData returnData;
        private FlowBreak breakCall;

        ///<summary>Short for 'new Flow()'</summary>
        public static Flow New { get { return new Flow(); } }

        ///<summary>Same as 'port.Call(f)'</summary>
        public void Call(FlowOutput port) {
            port.Call(this);
        }

        ///<summary>Read a temporary flow parameter</summary>
        public T ReadParameter<T>(string name) {
            object parameter = default(T);
            if ( parameters != null ) {
                parameters.TryGetValue(name, out parameter);
            }
            return parameter is T ? (T)parameter : default(T);
        }

        ///<summary>Write a temporary flow parameter</summary>
        public void WriteParameter<T>(string name, T value) {
            if ( parameters == null ) {
                parameters = new Dictionary<string, object>();
            }
            parameters[name] = value;
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Set Return Data to be calledback when Return is called</summary>
        public void SetReturnData(FlowReturn call, System.Type expectedType) {
            returnData = new ReturnData(call, expectedType);
        }

        ///<summary>Invoke Return callback with provided return value </summary>
        public void Return(object value, FlowNode context) {
            if ( returnData.returnCall == null ) {
                context.Fail("Called Return without anything to return out from.");
                return;
            }
            if ( returnData.returnType != null ) {
                var valueType = value != null ? value.GetType() : null;
                if ( valueType != null && !valueType.RTIsAssignableTo(returnData.returnType) ) {
                    context.Fail(string.Format("Return Value is not of expected type '{0}'", returnData.returnType.FriendlyName()));
                    return;
                }
            }
            if ( returnData.returnType == null && value != null ) {
                context.Warn("Returning a value when no value is required.");
            }
            returnData.returnCall(value);
        }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Start a break callback</summary>
        public void BeginBreakBlock(FlowBreak callback) {
            breakCall = callback;
        }

        ///<summary>End a break callback</summary>
        public void EndBreakBlock() {
            if ( breakCall == null ) {
                ParadoxNotion.Services.Logger.LogWarning("Called EndBreakBlock wihout a previously BeginBreakBlock call.", NodeCanvas.Framework.LogTag.EXECUTION);
                return;
            }
            breakCall = null;
        }

        ///<summary>Invoke Break callback.</summary>
        public void Break(FlowNode context) {
            if ( breakCall == null ) {
                context.Warn("Called Break without anything to break out from.");
                return;
            }
            breakCall();
        }

    }
}