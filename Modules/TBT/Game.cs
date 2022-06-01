namespace XiheFramework {
    public static partial class Game {
        public static GridModule Grid => GameManager.GetModule<GridModule>();
        public static NpcModule Npc => GameManager.GetModule<NpcModule>();

        

        //public static ChessBoardModule ChessBoard => GameManager.GetModule<ChessBoardModule>();

        //public static UnitModule Units => GameManager.GetModule<UnitModule>();

        //public static AIModule AI => GameManager.GetModule<AIModule>();

        //public static InstructionModule Instructions => GameManager.GetModule<InstructionModule>();

        //public static DirectorModule Director => GameManager.GetModule<DirectorModule>();

        //public static MapBuildingModule Building => GameManager.GetModule<MapBuildingModule>();

        //public static BuffModule Buff => GameManager.GetModule<BuffModule>();
    }
}