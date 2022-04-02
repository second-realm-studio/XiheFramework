using System.Reflection;
using System.Linq;
using ParadoxNotion;
using ParadoxNotion.Serialization;
using UnityEngine;


namespace FlowCanvas.Nodes
{
    ///<summary>Wraps a MethodInfo into a FlowGraph node</summary>
    public class ReflectedMethodNodeWrapper : ReflectedMethodBaseNodeWrapper
    {

        [SerializeField]
        private SerializedMethodInfo _method;

        private BaseReflectedMethodNode reflectedMethodNode { get; set; }
        private MethodInfo method => _method;
        protected override ISerializedMethodBaseInfo serializedMethodBase => _method;

        public override string name {
            get
            {
                if ( method != null ) {
                    var specialType = ReflectionTools.MethodType.Normal;
                    var methodName = method.FriendlyName(out specialType);
                    if ( specialType == ReflectionTools.MethodType.Operator ) {
                        ReflectionTools.op_FriendlyNamesShort.TryGetValue(method.Name, out methodName);
                        return methodName;
                    }
                    methodName = methodName.SplitCamelCase();
                    if ( method.IsGenericMethod ) {
                        methodName += string.Format(" ({0})", method.RTGetGenericArguments().First().FriendlyName());
                    }
                    if ( !method.IsStatic || method.IsExtensionMethod() ) {
                        return methodName;
                    }
                    return string.Format("{0}.{1}", method.DeclaringType.FriendlyName(), methodName);
                }
                if ( _method != null ) {
                    return _method.AsString().FormatError();
                }
                return "NOT SET";
            }
        }

        ///<summary>Set a new MethodInfo to be used by ReflectedMethodNode</summary>
        public override void SetMethodBase(MethodBase newMethod, object instance = null) {
            if ( newMethod is MethodInfo ) {
                SetMethod((MethodInfo)newMethod, instance);
            }
        }

        ///<summary>Set a new MethodInfo to be used by ReflectedMethodNode</summary>
        void SetMethod(MethodInfo newMethod, object instance = null) {

            //open generic
            if ( newMethod.IsGenericMethodDefinition ) {
                var wildType = newMethod.GetFirstGenericParameterConstraintType();
                newMethod = newMethod.MakeGenericMethod(wildType);
            }

            //drop hierarchy to base definition
            newMethod = newMethod.GetBaseDefinition();

            _method = new SerializedMethodInfo(newMethod);
            _callable = newMethod.ReturnType == typeof(void);
            GatherPorts();

            base.SetDropInstanceReference(newMethod, instance);
            base.SetDefaultParameterValues(newMethod);
        }

        ///<summary>When ports connects and is a generic method, try change method to that port type</summary>
        public override void OnPortConnected(Port port, Port otherPort) {
            if ( method.IsGenericMethod ) {
                var wildType = method.GetFirstGenericParameterConstraintType();
                var newMethod = FlowNode.TryGetNewGenericMethodForWild(wildType, port.type, otherPort.type, method);
                if ( newMethod != null ) {
                    _method = new SerializedMethodInfo(newMethod);
                    GatherPorts();
                }
            }
        }

        //...
        public override System.Type GetNodeWildDefinitionType() {
            return method.GetFirstGenericParameterConstraintType();
        }

        ///<summary>Gather the ports through the wrapper</summary>
        protected override void RegisterPorts() {
            if ( method == null ) {
                return;
            }

            var options = new ReflectedMethodRegistrationOptions();
            options.callable = callable;
            options.exposeParams = exposeParams;
            options.exposedParamsCount = exposedParamsCount;

            reflectedMethodNode = BaseReflectedMethodNode.GetMethodNode(method, options);
            if ( reflectedMethodNode != null ) {
                reflectedMethodNode.RegisterPorts(this, options);
            }
        }
    }
}