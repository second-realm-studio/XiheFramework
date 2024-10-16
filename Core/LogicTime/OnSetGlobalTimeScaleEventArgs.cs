namespace XiheFramework.Core.LogicTime {
    public struct OnSetGlobalTimeScaleEventArgs {
        public float newTimeScale;
        public float oldTimeScale;
        public float duration;
        
        public OnSetGlobalTimeScaleEventArgs(float newTimeScale, float oldTimeScale, float duration) {
            this.newTimeScale = newTimeScale;
            this.oldTimeScale = oldTimeScale;
            this.duration = duration;
        }
    }
}