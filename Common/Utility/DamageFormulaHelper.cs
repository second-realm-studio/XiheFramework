namespace XiheFramework {
    public static class DamageFormulaHelper {
        public static float GetDamage(float attack, float defense) {
            return attack * (100f / (100f + defense));
        }
    }
}