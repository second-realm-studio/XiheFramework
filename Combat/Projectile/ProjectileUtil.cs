namespace XiheFramework.Combat.Projectile {
    public static class ProjectileUtil {
        public static string GetProjectileEntityAddress(string projectileName) {
            return $"ProjectileEntity_{projectileName}";
        }
    }
}