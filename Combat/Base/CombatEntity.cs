using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XiheFramework.Combat.Action;
using XiheFramework.Combat.Buff;
using XiheFramework.Combat.Constants;
using XiheFramework.Combat.Damage.DataTypes;
using XiheFramework.Entry;

namespace XiheFramework.Combat.Base {
    public class CombatEntity : CombatEntityBase {
        public string combatEntityName;
        public float maxHp;
        public float maxStamina;
        public string entryActionName;

        public override string entityName {
            get {
                if (string.IsNullOrEmpty(combatEntityName)) {
                    combatEntityName = gameObject.name + " " + GetInstanceID();
                }

                return combatEntityName;
            }
        }

        #region Basic Stats

        private float m_CurrentHp;

        public float CurrentHp {
            get => m_CurrentHp;
            set {
                m_CurrentHp = value;
                m_CurrentHp = Mathf.Clamp(m_CurrentHp, 0, maxHp);
                XiheFramework.Entry.Game.Blackboard.SetData(GeneralBlackboardNames.CombatEntity_CurrentHp(this), m_CurrentHp);
            }
        }

        private float m_CurrentStamina;
        private float m_CachedCurrentStamina;

        public float CurrentStamina {
            get => m_CurrentStamina;
            set {
                m_CurrentStamina = value;
                m_CurrentStamina = Mathf.Clamp(m_CurrentStamina, 0, maxStamina);
                if (Mathf.Abs(m_CachedCurrentStamina - m_CurrentStamina) > 1f) {
                    XiheFramework.Entry.Game.Blackboard.SetData(GeneralBlackboardNames.CombatEntity_CurrentStamina(this), m_CurrentStamina);
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
                XiheFramework.Entry.Game.Blackboard.SetData(GeneralBlackboardNames.CombatEntity_CurrentLooking(this), value);
            }
        }

        #endregion

        #region Buffs

        private Dictionary<string, Buff.BuffEntity> m_BuffEntities = new Dictionary<string, Buff.BuffEntity>();
        public string[] BuffArray => m_BuffEntities.Keys.ToArray();

        public Buff.BuffEntity GetBuffEntity(string buffName) {
            return m_BuffEntities.ContainsKey(buffName) ? m_BuffEntities[buffName] : null;
        }

        public bool HasBuff(string buffName) => m_BuffEntities.ContainsKey(buffName);

        #endregion

        public Action.ActionEntity CurrentActionEntity { get; private set; }

        public CharacterController CharacterController { get; private set; }

        #region Entity Root Transforms

        [HideInInspector]
        public Transform actionRoot;

        [HideInInspector]
        public Transform animationRoot;

        [HideInInspector]
        public Transform buffRoot;

        [HideInInspector]
        public Transform particleRoot;

        #endregion

        protected override void Start() {
            base.Start();

            CharacterController = GetComponent<CharacterController>();

            //event
            XiheFramework.Entry.Game.Event.Subscribe(Game.Action.OnChangeActionEventName, OnChangeAction);
            XiheFramework.Entry.Game.Event.Subscribe(Game.Damage.OnProcessedDamageEventName, OnDamageProcessed);
            XiheFramework.Entry.Game.Event.Subscribe(Game.Buff.OnAddBuffEventName, OnAddBuff);
            XiheFramework.Entry.Game.Event.Subscribe(Game.Buff.OnRemoveBuffEventName, OnRemoveBuff);

            //load hp and stamina
            CurrentHp = maxHp;
            CurrentStamina = maxStamina;
            XiheFramework.Entry.Game.Blackboard.SetData(GeneralBlackboardNames.CombatEntity_MaxHp(this), maxHp);
            XiheFramework.Entry.Game.Blackboard.SetData(GeneralBlackboardNames.CombatEntity_MaxStamina(this), maxStamina);

            //load entry action
            Game.Action.ChangeAction(entityId, entryActionName);

            //entity spawn roots
            actionRoot = new GameObject("_ActionEntities").transform;
            actionRoot.SetParent(transform);
            actionRoot.localPosition = Vector3.zero;
            actionRoot.localRotation = Quaternion.identity;
            actionRoot.localScale = Vector3.one;

            animationRoot = new GameObject("_AnimationEntities").transform;
            animationRoot.SetParent(transform);
            animationRoot.localPosition = Vector3.zero;
            animationRoot.localRotation = Quaternion.identity;
            animationRoot.localScale = Vector3.one;

            buffRoot = new GameObject("_BuffEntities").transform;
            buffRoot.SetParent(transform);
            buffRoot.localPosition = Vector3.zero;
            buffRoot.localRotation = Quaternion.identity;
            buffRoot.localScale = Vector3.one;

            particleRoot = new GameObject("_ParticleEntities").transform;
            particleRoot.SetParent(transform);
            particleRoot.localPosition = Vector3.zero;
            particleRoot.localRotation = Quaternion.identity;
            particleRoot.localScale = Vector3.one;
        }

        private void OnChangeAction(object sender, object e) {
            if (sender is not uint target || (uint?)target != entityId) {
                return;
            }

            if (e is not OnChangeActionArgs newActionArgs) {
                return;
            }

            var action = Game.Action.LoadAction(newActionArgs.actionName);
            action.Init(this, newActionArgs.args);

            if (CurrentActionEntity) {
                CurrentActionEntity.Exit();
                StartCoroutine(UnloadLastActionAfterOneFrame(CurrentActionEntity));
            }

            CurrentActionEntity = action;
        }

        private IEnumerator UnloadLastActionAfterOneFrame(Action.ActionEntity action) {
            yield return null;
            action.Unload();
        }

        private void OnAddBuff(object sender, object e) {
            if (sender is not uint target || (uint?)target != entityId) {
                return;
            }

            var eventArgs = e as OnAddBuffEventArgs;
            if (eventArgs == null) {
                return;
            }

            if (!m_BuffEntities.ContainsKey(eventArgs.buffName)) {
                m_BuffEntities.Add(eventArgs.buffName, eventArgs.buffEntity);
            }

            m_BuffEntities[eventArgs.buffName].OnBuffAdd(this, eventArgs.deltaStack);
        }

        private void OnRemoveBuff(object sender, object e) {
            if (sender is not uint target || target != entityId) {
                return;
            }

            var eventArgs = e as OnRemoveBuffEventArgs;
            if (eventArgs == null) {
                return;
            }

            m_BuffEntities[eventArgs.buffName].OnBuffRemove(eventArgs.stack);
            if (m_BuffEntities[eventArgs.buffName] == null || m_BuffEntities[eventArgs.buffName].CurrentStack == 0) {
                m_BuffEntities.Remove(eventArgs.buffName);
            }
        }

        private void OnDamageProcessed(object sender, object e) {
            if (sender is not uint target || target != entityId) {
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