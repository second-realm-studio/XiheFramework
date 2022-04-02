#define DO_EDITOR_BINDING //comment this out to test the real performance without editor binding specifics

using System;
using System.Collections.Generic;
using System.Linq;
using ParadoxNotion;
using UnityEngine;

namespace FlowCanvas
{
    ///<summary>Base Port class for all port types</summary>
    [ParadoxNotion.Design.SpoofAOT]
    abstract public class Port
    {

#if UNITY_EDITOR
        ///<summary>Editor display name</summary>
        internal string displayName { get; private set; }
        ///<summary>Editor display content. Editor is port drawn in gui</summary>
        internal bool willDraw { get; set; }
        ///<summary>Editor port highlight flag</summary>
        internal bool highlightFlag { get; set; }
        ///<summary>Editor relative y offset of the port in relation to it's parent</summary>
        internal float posOffsetY { private get; set; }
        ///<summary>Editor position of port</summary>
        internal Vector2 pos => rect.center;
        ///<summary>Editor rect of port</summary>
        internal Rect rect {
            get
            {
                var result = new Rect(0, 0, 12, 12);
                var finalOffset = ( willDraw ? posOffsetY : parent.rect.height / 2 ) + 1;
                if ( IsInputPort() ) { result.center = new Vector2(parent.rect.xMin - 5, parent.rect.y + finalOffset); }
                if ( IsOutputPort() ) { result.center = new Vector2(parent.rect.xMax + 5, parent.rect.y + finalOffset); }
                return result;
            }
        }

        ///<summary>Editor GUI Caching of port</summary>
        internal void EnsureCachedGUIContent() {
            if ( displayContent != null ) { return; }
            var content = new GUIContent();
            displayName = name.SplitCamelCase();
            var displayNameCaped = displayName.CapLength(25);
            if ( IsFlowPort() ) {
                if ( this is FlowInput ) {
                    content.text = string.Format("<b>► {0}</b>", displayNameCaped);
                } else {
                    content.text = string.Format("<b>{0} ►</b>", displayNameCaped);
                }
            } else {
                var hexColor = ColorUtils.ColorToHex(ParadoxNotion.Design.TypePrefs.GetTypeColor(type));
                var text = string.Format("<color={0}>{1}{2}</color>", hexColor, IsEnumerableCollection() ? "#" : string.Empty, displayNameCaped);
                Texture icon = null;
                if ( NodeCanvas.Editor.Prefs.showIcons ) {
                    var wildDefType = parent.GetNodeWildDefinitionType();
                    var isWildDef = wildDefType != null && ( wildDefType == type || wildDefType == type.GetEnumerableElementType() );
                    icon = ParadoxNotion.Design.TypePrefs.GetTypeIcon(isWildDef ? typeof(Wild) : type);
                }

                if ( IsDelegate() ) {
                    text = string.Format("<b><color={0}>[{1}]</color></b>", hexColor, text);
                }

                content.text = text;
                content.image = icon;
            }

            content.tooltip = displayName.Length > displayNameCaped.Length ? displayName : string.Empty;

            if ( bindStatus != BindStatus.Valid ) {
                content.text = displayName.FormatError();
                content.image = ParadoxNotion.Design.Icons.errorIcon;
                content.tooltip = bindStatus.ToString();
            }

            displayContent = content;
        }
#endif


        ///----------------------------------------------------------------------------------------------

        ///<summary>required for serialization -> dont use</summary>
        public Port() { }

        public Port(FlowNode parent, string name, string ID) {
            this.parent = parent;
            this.name = name;
            this.ID = ID;
        }

        //Port current bind status
        public enum BindStatus
        {
            Valid = 0,
            Missing = 1,
            InvalidCast = 2,
        }

        ///<summary>The FlowNode parent reference of the port</summary>
        public FlowNode parent { get; private set; }
        ///<summary>The ID name of the port. Usualy is the same as the name</summary>
        public string ID { get; private set; }
        ///<summary>The name of the port</summary>
        public string name { get; private set; }
        ///<summary>The number of connections the port currently has</summary>
        public int connections { get; internal set; }
        ///<summary>Is the port connected?</summary>
        public bool isConnected { get { return connections > 0; } }
        ///<summary>The current binding status of the port</summary>
        internal BindStatus bindStatus { get; private set; }
        ///<summary>Display GUIContent</summary>
        internal GUIContent displayContent { get; private set; }

        ///<summary>The type of the port</summary>
        abstract public Type type { get; }

        ///----------------------------------------------------------------------------------------------

        ///<summary>Flags the port as a missing one</summary>
        internal Port FlagMissing() {
            displayContent = null; //flush
            bindStatus = BindStatus.Missing;
            return this;
        }

        ///<summary>Flags the port as invalid cast</summary>
        internal Port FlagInvalidCast() {
            displayContent = null; //flush
            bindStatus = BindStatus.InvalidCast;
            return this;
        }

        ///<summary>Flags the port as valid</summary>
        internal Port FlagValid() {
            displayContent = null; //flush
            bindStatus = BindStatus.Valid;
            return this;
        }

        ///<summary>Helper method to determine if a port can accept further connections</summary>
        public bool CanAcceptConnections() {
            if ( this is ValueOutput || ( this is FlowOutput && !this.isConnected ) ) { return true; }
            if ( this is FlowInput || ( this is ValueInput && !this.isConnected ) ) { return true; }
            return false;
        }

        ///<summary>Get all BinderConnections the port is using</summary>
        public IEnumerable<BinderConnection> GetPortConnections() {
            if ( IsInputPort() ) { return parent.inConnections.OfType<BinderConnection>().Where(c => c.targetPort == this); }
            return parent.outConnections.OfType<BinderConnection>().Where(c => c.sourcePort == this);
        }

        ///<summary>Gets the first input BinderConnection of the port</summary>
        public BinderConnection GetFirstInputConnection() {
            return parent.inConnections.OfType<BinderConnection>().FirstOrDefault(c => c.targetPort == this);
        }

        ///<summary>Gets the first output BinderConnection of the port</summary>
        public BinderConnection GetFirstOutputConnection() {
            return parent.outConnections.OfType<BinderConnection>().FirstOrDefault(c => c.sourcePort == this);
        }

        public bool IsFlowPort() {
            return ( this is FlowInput ) || ( this is FlowOutput );
        }

        public bool IsValuePort() {
            return ( this is ValueInput ) || ( this is ValueOutput );
        }

        public bool IsInputPort() {
            return ( this is FlowInput ) || ( this is ValueInput );
        }

        public bool IsOutputPort() {
            return ( this is FlowOutput ) || ( this is ValueOutput );
        }

        public bool IsUnityObject() {
            return typeof(UnityEngine.Object).RTIsAssignableFrom(type);
        }

        public bool IsUnitySceneObject() {
            return typeof(Component).RTIsAssignableFrom(type) || type == typeof(GameObject);
        }

        public bool IsDelegate() {
            return typeof(Delegate).RTIsAssignableFrom(type);
        }

        public bool IsEnumerableCollection() {
            return type.IsEnumerableCollection();
        }

        public bool IsWild() {
            return type == typeof(Wild);
        }

        public override string ToString() {
            return name.SplitCamelCase();
        }
    }


    ///----------------------------------------------------------------------------------------------
    //FLOW PORTS
    ///----------------------------------------------------------------------------------------------

    ///<summary>Input port for Flow type</summary>
    public class FlowInput : Port
    {

        ///<summary>pointer refers to callback when the port is called</summary>
        public FlowInput(FlowNode parent, string name, string ID, FlowHandler pointer) : base(parent, name, ID) {
            this.pointer = pointer;
        }

        ///<summary>Used for port binding. Points to the call for when port is called</summary>
        public FlowHandler pointer { get; private set; }
        ///<summary>The type of the port which is always type of Flow</summary>
        public override Type type { get { return typeof(Flow); } }
    }

    ///<summary>Output port for Flow type</summary>
    public class FlowOutput : Port
    {

        public FlowOutput(FlowNode parent, string name, string ID) : base(parent, name, ID) { }

        ///<summary>Used for port binding. Points to a FlowInput pointer if ports are connected</summary>
        public event FlowHandler pointer;
        ///<summary>The type of the port which is always type of Flow</summary>
        public override Type type { get { return typeof(Flow); } }

        ///<summary>Calls the target bound pointer</summary>
        public void Call(Flow f) {
            if ( pointer != null && parent.graph.isRunning ) {
                f.ticks++;

#if UNITY_EDITOR && DO_EDITOR_BINDING

                if ( parent.isBreakpoint ) {
                    ParadoxNotion.Services.MonoManager.current.StartCoroutine(DebugBreakWait(f));
                    return;
                }

                Continue(f);

#else

				pointer(f);

#endif

            }
        }

#if UNITY_EDITOR && DO_EDITOR_BINDING //Breakpoint helpers
        System.Collections.IEnumerator DebugBreakWait(Flow f) {
            ParadoxNotion.Services.Logger.LogWarning(string.Format("On Node '{0}'", parent.name), "Breakpoint", parent);
            UnityEngine.Debug.Break();
            parent.SetStatus(NodeCanvas.Framework.Status.Running);
            yield return null;
            parent.SetStatus(NodeCanvas.Framework.Status.Resting);
            Continue(f);
        }

        void Continue(Flow f) {
            try { pointer(f); }
            catch ( Exception e ) {
                var connection = GetFirstOutputConnection();
                var targetNode = (FlowNode)connection.targetNode;
                targetNode.Error(e);
            }
        }
#endif


        ///<summary>Bind the port to the target FlowInput</summary>
        public void BindTo(FlowInput target) {
            this.pointer = target.pointer;
        }

        ///<summary>Appends a callback when port is called</summary>
        public void Append(FlowHandler callback) {
            this.pointer += callback;
        }

        ///<summary>Unbinds the port</summary>
        public void UnBind() {
            this.pointer = null;
        }
    }


    ///----------------------------------------------------------------------------------------------
    //VALUE INPUT PORT
    ///----------------------------------------------------------------------------------------------

    ///<summary>Base input port for values</summary>
    abstract public class ValueInput : Port
    {

        public ValueInput(FlowNode parent, string name, string ID) : base(parent, name, ID) { }

        ///<summary>Creates a generic instance of ValueInput</summary>
        public static ValueInput<T> CreateInstance<T>(FlowNode parent, string name, string ID) {
            return new ValueInput<T>(parent, name, ID);
        }

        ///<summary>Creates a generic instance of ValueInput</summary>
        public static ValueInput CreateInstance(Type t, FlowNode parent, string name, string ID) {
            return (ValueInput)Activator.CreateInstance(typeof(ValueInput<>).RTMakeGenericType(t), new object[] { parent, name, ID });
        }

        ///<summary>The value as object type when accessed as ValueInput</summary>
        public object value {
            get { return GetObjectValue(); }
        }

        ///<summary>Convenience method to set default port value and serialized port value, which should be the same value in any case.</summary>
        public ValueInput SetDefaultAndSerializedValue(object v) {
            this.defaultValue = v;
            this.serializedValue = v;
            return this;
        }

        ///<summary>Convenience method to make the port skip self object assigment if it was a valid candidate in the first place.</summary>
        public ValueInput SkipSelfInstanceAssignment(bool skip) {
            this.skipSelfInstanceAssignment = skip;
            return this;
        }

        public bool skipSelfInstanceAssignment { get; private set; }
        abstract public object defaultValue { get; set; }
        abstract public object serializedValue { get; set; }
        abstract public bool isDefaultValue { get; }
        abstract public override Type type { get; }
        abstract public void BindTo(ValueOutput target);
        abstract public void UnBind();
        abstract public object GetObjectValue();
    }

    ///<summary>Value Input port for a known type. .value refers to either connected binded port if any, or the serialized exposed value</summary>
    public class ValueInput<T> : ValueInput
    {

        public ValueInput(FlowNode parent, string name, string ID) : base(parent, name, ID) { }

        ///<summary>Used for port binding</summary>
        public event ValueHandler<T> getter;
        private Action<T> callback;

        private T _value;
        private T _defaultValue;

        ///<summary>The port value</summary>
        new public T value {
            get
            {
                T result = _value;
                if ( getter != null ) {

#if UNITY_EDITOR && DO_EDITOR_BINDING

                    try { result = getter(); }
                    catch ( Exception e ) {
                        var connection = GetFirstInputConnection();
                        var targetNode = (FlowNode)connection.sourceNode;
                        targetNode.Error(e);
                    }
#else
					result = getter();
#endif
                }

                if ( callback != null ) {
                    callback(result);
                }
                return result;
            }
        }

        ///<summary>The value which is considered default</summary>
        public override object defaultValue {
            get { return _defaultValue; }
            set { _defaultValue = (T)value; }
        }

        ///<summary>Used to get/set the serialized value directly</summary>
        public override object serializedValue {
            get { return _value; }
            set { _value = (T)value; }
        }

        ///<summary>Returns if the serializedValue is equal to the default value, usually simply default(T)</summary>
        public override bool isDefaultValue {
            get { return ObjectUtils.AnyEquals(serializedValue, defaultValue); }
        }

        ///<summary>The port value type which is always of type T</summary>
        public override Type type { get { return typeof(T); } }

        ///<summary>Sets the default and serialized value of the port. Can be used when registering ports.</summary>
        public ValueInput<T> SetDefaultAndSerializedValue(T v) {
            this._defaultValue = v;
            this._value = v;
            return this;
        }

        ///<summary>Get the value same as .value (can be used for delegate since properties can't directly)</summary>
        public T GetValue() {
            return value;
        }

        ///<summary>Get the value as object</summary>
        public override object GetObjectValue() {
            return value;
        }

        ///<summary>Binds the port to the target source ValueOutput port</summary>
        public override void BindTo(ValueOutput source) {
            //if same T use directly
            if ( source is ValueOutput<T> ) {
                this.getter = ( source as ValueOutput<T> ).getter;
                return;
            }

            //otherwise use the converter
            this.getter = TypeConverter.GetConverterFuncFromTo<T>(source.type, typeof(T), source.GetObjectValue);
        }

        ///<summary>Append callback when getter is called with the result value</summary>
        public void Append(Action<T> callback) {
            this.callback += callback;
        }

        ///<summary>Unbinds the port</summary>
        public override void UnBind() {
            this.getter = null;
            this.callback = null;
        }

        //operator
        public static explicit operator T(ValueInput<T> port) {
            return port.value;
        }
    }


    ///----------------------------------------------------------------------------------------------
    //VALUE OUTPUT PORT
    ///----------------------------------------------------------------------------------------------

    ///<summary>Base output port for values</summary>
    abstract public class ValueOutput : Port
    {

        public ValueOutput(FlowNode parent, string name, string ID) : base(parent, name, ID) { }

        ///<summary>Creates a generic instance of ValueOutput</summary>
        public static ValueOutput<T> CreateInstance<T>(FlowNode parent, string name, string ID, ValueHandler<T> getter) {
            return new ValueOutput<T>(parent, name, ID, getter);
        }

        ///<summary>Creates a generic instance of ValueOutput</summary>
        public static ValueOutput CreateInstance(Type t, FlowNode parent, string name, string ID, ValueHandlerObject getter) {
            return (ValueOutput)Activator.CreateInstance(typeof(ValueOutput<>).RTMakeGenericType(t), new object[] { parent, name, ID, getter });
        }

        ///<summary>Used only in case that a binder required casting cause of different port types (a conversion)</summary>
        abstract public object GetObjectValue();
    }

    ///<summary>An output value port</summary>
    public class ValueOutput<T> : ValueOutput
    {

        //normal
        public ValueOutput(FlowNode parent, string name, string ID, ValueHandler<T> getter) : base(parent, name, ID) {
            this.getter = getter;
        }

        //casted
        public ValueOutput(FlowNode parent, string name, string ID, ValueHandlerObject getter) : base(parent, name, ID) {
            this.getter = () => { return (T)getter(); };
        }

        ///<summary>Used for port binding</summary>
        public ValueHandler<T> getter { get; private set; }

        ///<summary>Used only in case that a binder required casting cause of different port types</summary>
        public override object GetObjectValue() { return (object)getter(); }
        ///<summary>The type of the port which is always of type T</summary>
        public override Type type { get { return typeof(T); } }

        //operator
        public static explicit operator T(ValueOutput<T> port) {
            return port.getter();
        }
    }
}