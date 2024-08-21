using XiheFramework.Combat.Action;
using XiheFramework.Combat.Animation2D;
using XiheFramework.Combat.Buff;
using XiheFramework.Combat.Damage;
using XiheFramework.Combat.Damage.Interfaces;
using XiheFramework.Combat.Particle;
using XiheFramework.Combat.Projectile;
using XiheFramework.Core.Base;

namespace XiheFramework.Runtime {
    public static partial class Game {
        public static BuffModule Buff { get; internal set; }
        public static ActionModule Action { get; internal set; }
        public static Animation2DModule Animation2D { get; internal set; }
        public static IDamageModule Damage { get; internal set; }
        public static ParticleModule Particle { get; internal set; }
        public static ProjectileModule Projectile { get; internal set; }
    }
}