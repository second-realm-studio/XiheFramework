namespace XiheFramework {
    public static partial class Game {
        public static InteractionModule Interaction => GameManager.GetModule<InteractionModule>();

        public static HitModule Hit => GameManager.GetModule<HitModule>();
    }
}