namespace XiheFramework.Combat.Particle {
    public static class ParticleUtil {
        public static string GetParticleEntityAddress(string particleName) {
            return $"ParticleEntity_{particleName}";
        }
    }
}