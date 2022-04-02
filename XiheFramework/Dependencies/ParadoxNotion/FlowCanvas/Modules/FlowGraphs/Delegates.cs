namespace FlowCanvas
{

    ///<summary>Delegate for Flow</summary>
    public delegate void FlowHandler(Flow f);
    ///<summary>Delegate for Values</summary>
    [ParadoxNotion.Design.SpoofAOT]
    public delegate T ValueHandler<T>();
    ///<summary>Delegate for object casted Values only</summary>
    public delegate object ValueHandlerObject();
    ///<summary>Delegate for Flow Loop Break</summary>
    public delegate void FlowBreak();
    ///<summary>Delegate for Flow Function Return</summary>
    public delegate void FlowReturn(object value);
}