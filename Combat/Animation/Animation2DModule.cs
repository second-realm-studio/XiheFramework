using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Combat.Base;
using XiheFramework.Core.Base;

namespace XiheFramework.Combat.Animation {
    public class Animation2DModule : GameModule {
        public readonly string OnAnimationCreated = "Animation2D.OnAnimationCreated";
        public readonly string OnAnimationPlayed = "Animation2D.OnAnimationPlayed";

        public void PlayAsync(CombatEntity ownerActionEntity, string animationName, int frameInterval, EndBehaviour endBehaviour, Vector3 localPosition,
            Vector3 localScale, System.Action<Animation2DEntity> onLoadedCallback) {
            //Debug.Log($"{ownerActionEntity.name} play animation {animationName}");

            var animationAddress = AnimationUtil.GetAnimation2DEntityAddress(animationName);
            XiheFramework.Entry.Game.Resource.InstantiateAssetAsync<GameObject>(animationAddress, go => {
                var entity = go.GetComponent<Animation2DEntity>();
                if (ownerActionEntity == null) {
                    Debug.LogWarning($"ownerActionEntity has been destroyed, destroying animation entity {gameObject.name}");
                    DestroyAnimation(entity);
                    return;
                }

                entity.Setup(ownerActionEntity, localPosition, localScale);
                onLoadedCallback?.Invoke(entity);
                entity.Play(endBehaviour, frameInterval);
            });
        }

        public void PlayAsync(CombatEntity ownerActionEntity, string animationName, int frameInterval, EndBehaviour endBehaviour,
            System.Action<Animation2DEntity> onLoadedCallback) {
            PlayAsync(ownerActionEntity, animationName, frameInterval, endBehaviour, Vector3.zero, Vector3.one, onLoadedCallback);
        }

        public Animation2DEntity Create(CombatEntity owner, string animationName, Vector3 localPosition, Vector3 localScale, bool playFirstFrameAtStart = false) {
            var animationAddress = AnimationUtil.GetAnimation2DEntityAddress(animationName);
            var go = XiheFramework.Entry.Game.Resource.InstantiateAsset<GameObject>(animationAddress);
            var entity = go.GetComponent<Animation2DEntity>();
            entity.Setup(owner, localPosition, localScale, playFirstFrameAtStart);
            return entity;
        }

        public Animation2DEntity Create(CombatEntity owner, string animationName, bool playFirstFrameAtStart = false) {
            var animationAddress = AnimationUtil.GetAnimation2DEntityAddress(animationName);
            var go = XiheFramework.Entry.Game.Resource.InstantiateAsset<GameObject>(animationAddress);
            var entity = go.GetComponent<Animation2DEntity>();
            entity.Setup(owner, Vector3.zero, Vector3.one, playFirstFrameAtStart);
            return entity;
        }

        public Animation2DEntity CreateAndPlay(CombatEntity ownerActionEntity, string animationName, int frameInterval, EndBehaviour endBehaviour,
            bool playFirstFrameAtStart = false) {
            return CreateAndPlay(ownerActionEntity, animationName, frameInterval, endBehaviour, Vector3.zero, Vector3.one, playFirstFrameAtStart);
        }

        public Animation2DEntity CreateAndPlay(CombatEntity ownerActionEntity, string animationName, int frameInterval, EndBehaviour endBehaviour, Vector3 localPosition,
            Vector3 localScale, bool playFirstFrameAtStart = false) {
            var anim = Create(ownerActionEntity, animationName, localPosition, localScale, playFirstFrameAtStart);
            anim.Play(endBehaviour, frameInterval);
            return anim;
        }

        public void SetAnimationInvisible(Animation2DEntity entity, bool visible = false) {
            if (entity == null) {
                return;
            }

            entity.mainMeshRenderer.enabled = visible;
        }

        public void RegisterEvent(Animation2DEntity entity, int frame, System.Action callback) {
            if (entity == null) {
                Debug.LogError("Animation2DModule.RegisterEvent: entity is null");
                return;
            }

            entity.RegisterEvent(frame, callback);
        }

        public bool TryGetAnimation2DEntityFrom(CombatEntity owner, string animationName, out Animation2DEntity entity) {
            foreach (var child in owner.animationRoot) {
                var trans = child as Transform;
                if (trans != null && trans.TryGetComponent(out Animation2DEntity anim)) {
                    if (anim.entityName == animationName) {
                        entity = anim;
                        return true;
                    }
                }
            }

            entity = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="entity"></param>
        /// <returns> return false when no animation2d entities found </returns>
        public bool TryGetAllAnimation2DEntityFrom(CombatEntity owner, out Animation2DEntity[] entity) {
            var result = new List<Animation2DEntity>();
            foreach (var child in owner.animationRoot) {
                var trans = child as Transform;
                if (trans != null && trans.TryGetComponent(out Animation2DEntity anim)) {
                    result.Add(anim);
                }
            }

            if (result.Count > 0) {
                entity = result.ToArray();
                return true;
            }

            entity = null;
            return false;
        }

        public void DestroyAnimation(Animation2DEntity entity) {
            if (entity == null) {
                return;
            }

            entity.StopAndDestroy();
        }
    }
}