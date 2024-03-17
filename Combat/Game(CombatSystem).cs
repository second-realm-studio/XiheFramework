using XiheFramework.Combat.Action;
using XiheFramework.Combat.Animation;
using XiheFramework.Combat.Buff;
using XiheFramework.Combat.Damage;
using XiheFramework.Combat.Dashable;
using XiheFramework.Combat.Interact;
using XiheFramework.Combat.Particle;
using XiheFramework.Combat.Projectile;
using XiheFramework.Core.Base;

namespace XiheFramework.Entry {
    public static partial class Game {
        public static BuffModule Buff => GameManager.GetModule<BuffModule>();
        public static DamageModule Damage => GameManager.GetModule<DamageModule>();
        public static ActionModule Action => GameManager.GetModule<ActionModule>();
        public static Animation2DModule Animation2D => GameManager.GetModule<Animation2DModule>();
        
        public static ParticleModule Particle => GameManager.GetModule<ParticleModule>();
        public static ProjectileModule Projectile => GameManager.GetModule<ProjectileModule>();
        public static InteractModule Interact => GameManager.GetModule<InteractModule>();
        public static DashableModule Dashable => GameManager.GetModule<DashableModule>();
    }
}