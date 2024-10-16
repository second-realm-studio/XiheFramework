using System;
using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Animation2D {
    public class Animation2DModule : GameModule {
        public readonly string onAnimationCreate = "Animation2D.OnAnimationCreate";
        public readonly string onAnimationPlay = "Animation2D.OnAnimationPlay";
        public readonly string onAnimationRegisterEvent = "Animation2D.OnAnimationRegisterEvent";
        public readonly string onAnimationUnregisterEvent = "Animation2D.OnAnimationUnregisterEvent";
        public readonly string onAnimationDestroy = "Animation2D.OnAnimationDestroy";

        /// <summary>
        /// Instantiate Animation at local position 0,0,0 and scale 1,1,1
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="animationAddress"></param>
        /// <param name="onLoadedCallback"></param>
        /// <param name="playFirstFrameAtStart"></param>
        public void CreateAnimationEntityAsync(uint ownerId, string animationAddress, System.Action<Animation2DEntity> onLoadedCallback, bool playFirstFrameAtStart = false) {
            Game.Entity.InstantiateEntityAsync<Animation2DEntity>(animationAddress, ownerId, true, 0, entity => {
                entity.Setup(playFirstFrameAtStart);
                onLoadedCallback?.Invoke(entity);
                Game.Event.Invoke(onAnimationCreate, ownerId, entity.EntityId);
            });
        }

        public void PlayAnimationAsync(uint ownerId, string animationAddress, int frameInterval, EndBehaviour endBehaviour, bool playFirstFrameAtStart = false,
            Action<Animation2DEntity> onLoadedCallback = null) {
            CreateAnimationEntityAsync(ownerId, animationAddress, entity => {
                onLoadedCallback?.Invoke(entity);
                entity.Play(endBehaviour, frameInterval);
                Game.Event.Invoke(onAnimationPlay, ownerId, entity.EntityId);
            }, playFirstFrameAtStart);
        }

        /// <summary>
        /// Instantiate Animation at local position 0,0,0 and scale 1,1,1
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="animationAddress"></param>
        /// <param name="playFirstFrameAtStart"></param>
        /// <returns></returns>
        public Animation2DEntity InstantiateAnimationEntity(uint ownerId, string animationAddress, bool playFirstFrameAtStart = false) {
            var animation2DEntity = Game.Entity.InstantiateEntity<Animation2DEntity>(animationAddress, ownerId, true, 0u, entity => {
                entity.Setup(playFirstFrameAtStart);
                Game.Event.Invoke(onAnimationCreate, ownerId, entity.EntityId);
            });

            return animation2DEntity;
        }

        /// <summary>
        /// Create and Play Animation at local position 0,0,0 and scale 1,1,1
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="animationAddress"></param>
        /// <param name="frameInterval"></param>
        /// <param name="endBehaviour"></param>
        /// <param name="playFirstFrameAtStart"></param>
        /// <returns></returns>
        public Animation2DEntity PlayAnimation(uint ownerId, string animationAddress, int frameInterval, EndBehaviour endBehaviour, bool playFirstFrameAtStart = false) {
            var anim = InstantiateAnimationEntity(ownerId, animationAddress, playFirstFrameAtStart);
            anim.Play(endBehaviour, frameInterval);
            return anim;
        }

        public void SetAnimationInvisible(uint entityId, bool visible = false) {
            var animation2D = Game.Entity.GetEntity<Animation2DEntity>(entityId);
            if (animation2D == null) {
                Debug.LogError("[Animation2D]SetAnimationInvisible: animation2D is null");
                return;
            }

            animation2D.mainMeshRenderer.enabled = visible;
        }

        public void RegisterEvent(uint animEntityId, int frame, System.Action callback) {
            var entity = Game.Entity.GetEntity<Animation2DEntity>(animEntityId);
            if (entity == null) {
                Debug.LogError("[Animation2D]RegisterEvent: entity is null");
                return;
            }

            entity.RegisterEvent(frame, callback);
            Game.Event.Invoke(onAnimationRegisterEvent, entity.OwnerId, entity.EntityId);
        }

        public void UnregisterEvent(uint animEntityId, int frame) {
            var entity = Game.Entity.GetEntity<Animation2DEntity>(animEntityId);
            if (entity == null) {
                Debug.LogError("[Animation2D]UnregisterEvent: entity is null");
                return;
            }

            entity.UnregisterAllEventsAt(frame);
            Game.Event.Invoke(onAnimationUnregisterEvent, entity.OwnerId, entity.EntityId);
        }

        public void DestroyAnimation(uint animationEntityId) {
            var entity = Game.Entity.GetEntity<Animation2DEntity>(animationEntityId);
            if (entity == null) {
                return;
            }

            var ownerId = entity.OwnerId;
            var entityId = entity.EntityId;
            Game.Entity.DestroyEntity(animationEntityId);
            Game.Event.Invoke(onAnimationDestroy, ownerId, entityId);
        }

        protected override void Awake() {
            base.Awake();
            Game.Animation2D = this;
        }
    }
}