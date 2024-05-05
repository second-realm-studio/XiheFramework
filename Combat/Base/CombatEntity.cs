using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XiheFramework.Combat.Action;
using XiheFramework.Combat.Animation2D;
using XiheFramework.Combat.Buff;
using XiheFramework.Combat.Constants;
using XiheFramework.Combat.Damage.DataTypes;
using XiheFramework.Runtime;

namespace XiheFramework.Combat.Base {
    public sealed class CombatEntity : CombatEntityBase {
        public string combatEntityName;
        public float maxHp;
        public float maxStamina;
        public string entryActionName;

        public override string EntityName {
            get {
                if (string.IsNullOrEmpty(combatEntityName)) {
                    combatEntityName = gameObject.name + " " + GetInstanceID();
                }

                return combatEntityName;
            }
        }

        #region Basic Stats

        private float m_CurrentHp;
        private float m_CachedCurrentHp; //buffer the change until it's greater than 1 to reduce the frequency of setting blackboard data

        public float CurrentHp {
            get => m_CurrentHp;
            set {
                m_CurrentHp = value;
                m_CurrentHp = Mathf.Clamp(m_CurrentHp, 0, maxHp);
                if (Mathf.Abs(m_CachedCurrentHp - m_CurrentHp) > 1f) {
                    Game.Blackboard.SetData(GeneralBlackboardNames.CombatEntity_CurrentHp(this), m_CurrentHp);
                    m_CachedCurrentHp = m_CurrentHp;
                }
            }
        }

        private float m_CurrentStamina;
        private float m_CachedCurrentStamina; //buffer the change until it's greater than 1 to reduce the frequency of setting blackboard data

        public float CurrentStamina {
            get => m_CurrentStamina;
            set {
                m_CurrentStamina = value;
                m_CurrentStamina = Mathf.Clamp(m_CurrentStamina, 0, maxStamina);
                if (Mathf.Abs(m_CachedCurrentStamina - m_CurrentStamina) > 1f) {
                    Game.Blackboard.SetData(GeneralBlackboardNames.CombatEntity_CurrentStamina(this), m_CurrentStamina);
                    m_CachedCurrentStamina = m_CurrentStamina;
                }
            }
        }

        public Vector3 CurrentPosition => transform.position;

        private Vector3 m_CurrentScale;

        public Vector3 CurrentScale => transform.localScale;

        private bool m_CurrentLooking; //right: true, left: false

        public bool CurrentLooking {
            get => m_CurrentLooking;
            set {
                m_CurrentLooking = value;
                Game.Blackboard.SetData(GeneralBlackboardNames.CombatEntity_CurrentLooking(this), value);
            }
        }

        #endregion

        #region Action

        public ActionEntity CurrentActionEntity { get; private set; }

        #endregion

        #region Animations

        private List<uint> m_AnimationEntities = new List<uint>();

        public uint[] AnimationArray => m_AnimationEntities.ToArray();

        #endregion

        #region Buffs

        private List<uint> m_BuffEntities = new List<uint>();

        public uint[] BuffArray => m_BuffEntities.ToArray();

        #endregion

        public CharacterController CharacterController { get; private set; }

        public override void OnInitCallback() {
            CharacterController = GetComponent<CharacterController>();

            //event
            Game.Event.Subscribe(Game.Action.onChangeActionEventName, OnChangeAction);
            Game.Event.Subscribe(Game.Damage.onProcessedDamageEventName, OnDamageProcessed);
            Game.Event.Subscribe(Game.Animation2D.onAnimationCreate, OnAnimationCreated);
            Game.Event.Subscribe(Game.Animation2D.onAnimationDestroy, OnAnimationDestroyed);
            Game.Event.Subscribe(Game.Buff.onBuffCreatedEvtName, OnAddBuff);
            Game.Event.Subscribe(Game.Buff.onBuffDestroyedEvtName, OnRemoveBuff);

            //load hp and stamina
            CurrentHp = maxHp;
            CurrentStamina = maxStamina;
            Game.Blackboard.SetData(GeneralBlackboardNames.CombatEntity_MaxHp(this), maxHp);
            Game.Blackboard.SetData(GeneralBlackboardNames.CombatEntity_MaxStamina(this), maxStamina);

            //load entry action
            Game.Action.ChangeAction(EntityId, entryActionName);
        }

        private void OnAnimationDestroyed(object sender, object e) {
            if (sender is not uint target || (uint?)target != EntityId) {
                return;
            }

            if (e is not uint animationEntityId) {
                return;
            }

            if (m_AnimationEntities.Contains(animationEntityId)) {
                m_AnimationEntities.Remove(animationEntityId);
            }
        }

        private void OnAnimationCreated(object sender, object e) {
            if (sender is not uint target || (uint?)target != EntityId) {
                return;
            }

            if (e is not uint animationEntityId) {
                return;
            }

            m_AnimationEntities.Add(animationEntityId);
        }
        
        private void OnChangeAction(object sender, object e) {
            if (sender is not uint target || (uint?)target != EntityId) {
                return;
            }

            if (e is not OnChangeActionArgs actionArgs) {
                return;
            }

            if (CurrentActionEntity) {
                //add unload queue
                Game.Entity.DestroyEntity(CurrentActionEntity.EntityId);
            }

            CurrentActionEntity = Game.Entity.GetEntity<ActionEntity>(actionArgs.actionEntityId);
        }

        private void OnAddBuff(object sender, object e) {
            if (sender is not uint target || (uint?)target != EntityId) {
                return;
            }

            var eventArgs = e as OnAddBuffEventArgs;
            if (eventArgs == null) {
                return;
            }

            if (!m_BuffEntities.Contains(eventArgs.buffEntityId)) {
                m_BuffEntities.Add(eventArgs.buffEntityId);
            }
        }

        private void OnRemoveBuff(object sender, object e) {
            if (sender is not uint target || target != EntityId) {
                return;
            }

            var args = e as OnRemoveBuffEventArgs;
            if (args == null) return;

            if (m_BuffEntities.Contains(args.buffEntityId)) {
                m_BuffEntities.Remove(args.buffEntityId);
            }
        }

        private void OnDamageProcessed(object sender, object e) {
            if (sender is not uint target || target != EntityId) {
                return;
            }

            var args = (DamageEventArgs)e;

            CurrentHp -= args.healthDamage;
            CurrentStamina -= args.staminaDamage;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.1f);

            if (CurrentLooking) {
                //draw line to right
                Gizmos.color = Color.green;
                Gizmos.DrawLine(CurrentPosition + Vector3.up, CurrentPosition + Vector3.right + Vector3.up);
            }
            else {
                //draw line to left
                Gizmos.color = Color.green;
                Gizmos.DrawLine(CurrentPosition + Vector3.up, CurrentPosition + Vector3.left + Vector3.up);
            }
        }
#endif
    }
}