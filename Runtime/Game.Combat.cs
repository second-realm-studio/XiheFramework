using XiheFramework.Combat.Action;
using XiheFramework.Combat.Animation2D;
using XiheFramework.Combat.Buff;
using XiheFramework.Combat.Damage;
using XiheFramework.Combat.Interact;
using XiheFramework.Combat.Particle;
using XiheFramework.Combat.Projectile;
using XiheFramework.Core.Base;

namespace XiheFramework.Runtime {
    public static partial class Game {
        public static BuffModule Buff => GameManager.GetModule<BuffModule>();
        public static DamageModule Damage => GameManager.GetModule<DamageModule>();
        public static ActionModule Action => GameManager.GetModule<ActionModule>();
        public static Animation2DModule Animation2D => GameManager.GetModule<Animation2DModule>();

        public static ParticleModule Particle => GameManager.GetModule<ParticleModule>();
        public static ProjectileModule Projectile => GameManager.GetModule<ProjectileModule>();
        public static InteractModule Interact => GameManager.GetModule<InteractModule>();
    }
}