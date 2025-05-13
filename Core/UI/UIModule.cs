using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using XiheFramework.Core.Base;
using XiheFramework.Core.UI.UIEntity;
using XiheFramework.Runtime;

namespace XiheFramework.Core.UI {
    public class UIModule : GameModuleBase {
        public override int Priority => (int)CoreModulePriority.UI;
        public Vector2 referenceResolution = new(1920, 1080);
        public float referencePixelsPerUnit = 100;
        private Canvas m_PageCanvas;
        private Canvas m_PopCanvas;
        private Canvas m_OverlayCanvas;

        private readonly LinkedList<PageReturnHistoryInfo> m_PageReturnHistory = new();
        private readonly Dictionary<uint, UILayoutEntityBase> m_PopEntities = new();
        private readonly Dictionary<uint, UILayoutEntityBase> m_OverlayEntities = new();

        public PageReturnHistoryInfo[] CurrentPageHistory => m_PageReturnHistory.ToArray();
        public UILayoutEntityBase[] CurrentPops => m_PopEntities.Values.ToArray();
        public UILayoutEntityBase[] CurrentOverlays => m_OverlayEntities.Values.ToArray();

        /// <summary>
        /// Open new UIPageEntity in Page Layer
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public uint OpenPage(string address) {
            if (!m_PageCanvas || string.IsNullOrEmpty(address)) return 0;
            HideOrDestroyPage();

            RemoveRepeatedHistory(address);

            var newUIEntity = InstantiateUILayoutEntity<UIPageEntityBase>(address, m_PageCanvas);
            if (newUIEntity.homePage) {
                var current = m_PageReturnHistory.First;
                while (current != null) {
                    if (current.Value.entityBase != null) {
                        Game.Entity.DestroyEntity(current.Value.entityBase.EntityId);
                    }

                    var next = current.Next;
                    m_PageReturnHistory.Remove(current);
                    current = next;
                }
            }

            newUIEntity.Show();

            var returnHistoryInfo = new PageReturnHistoryInfo(address, newUIEntity);
            m_PageReturnHistory.AddLast(returnHistoryInfo);

            return newUIEntity.EntityId;
        }

        /// <summary>
        /// Open Previous UILayoutEntity in Page Layer
        /// </summary>
        /// <returns></returns>
        public uint ReturnPrevPage() {
            if (!m_PageCanvas) return 0;
            if (m_PageReturnHistory.Count == 0) return 0;

            var entry = m_PageReturnHistory.Last.Value;
            if (entry.entityBase.homePage) {
                return entry.entityBase.EntityId;
            }

            Game.Entity.DestroyEntity(entry.entityBase.EntityId);
            m_PageReturnHistory.RemoveLast();

            if (m_PageReturnHistory.Count == 0) {
                return 0;
            }

            //find hidden entity
            var lastHistory = m_PageReturnHistory.Last.Value;
            var hiddenEntity = lastHistory.entityBase;
            if (hiddenEntity != null) {
                hiddenEntity.Show();
                return hiddenEntity.EntityId;
            }

            var newUIEntity = InstantiateUILayoutEntity<UIPageEntityBase>(lastHistory.address, m_PageCanvas);

            lastHistory = new PageReturnHistoryInfo(lastHistory.address, newUIEntity);
            m_PageReturnHistory.Last.Value = lastHistory;

            return newUIEntity.EntityId;
        }

        /// <summary>
        /// Open Home UILayoutEntity in Page Layer
        /// </summary>
        /// <returns></returns>
        public uint HomePage() {
            if (!m_PageCanvas) return 0;
            if (m_PageReturnHistory.Count == 0) return 0;
            if (m_PageReturnHistory.Last == null) {
                m_PageReturnHistory.Clear();
                return 0;
            }

            var current = m_PageReturnHistory.Last;
            while (current != null && current != m_PageReturnHistory.First) {
                if (current.Value.entityBase != null) {
                    Game.Entity.DestroyEntity(current.Value.entityBase.EntityId);
                }

                var next = current.Previous;
                m_PageReturnHistory.Remove(current);
                current = next;
            }

            var entry = m_PageReturnHistory.First.Value;
            UIPageEntityBase homeEntityBase;
            if (entry.entityBase != null) {
                homeEntityBase = entry.entityBase;
            }
            else {
                var newUIEntity = InstantiateUILayoutEntity<UIPageEntityBase>(entry.address, m_PageCanvas);

                entry = new PageReturnHistoryInfo(entry.address, newUIEntity);
                m_PageReturnHistory.First.Value = entry;
                homeEntityBase = newUIEntity;
            }

            homeEntityBase.Show();

            if (!homeEntityBase.homePage) {
                Debug.LogWarning($"[UI] Home Page not found! Opening first page in history: {entry.address}");
            }

            return homeEntityBase.EntityId;
        }

        private void RemoveRepeatedHistory(string address) {
            if (m_PageReturnHistory.Count == 0) return;

            //if opening same page, refresh page only, don't need to remove history
            if (m_PageReturnHistory.Last.Value.address == address) {
                var hiddenEntity = m_PageReturnHistory.Last.Value.entityBase;
                if (hiddenEntity != null) {
                    Game.Entity.DestroyEntity(hiddenEntity.EntityId);
                }

                m_PageReturnHistory.RemoveLast();
                return;
            }

            var currentNode = m_PageReturnHistory.Last;
            while (currentNode != null) {
                if (currentNode.Value.address == address) {
                    var temp = currentNode;
                    while (temp != null) {
                        if (temp.Value.entityBase != null) {
                            Game.Entity.DestroyEntity(temp.Value.entityBase.EntityId);
                        }

                        var prev = temp.Previous;
                        m_PageReturnHistory.Remove(temp);
                        temp = prev;
                    }

                    break;
                }

                currentNode = currentNode.Previous;
            }
        }

        private void HideOrDestroyPage() {
            if (m_PageReturnHistory.Count == 0) return;
            var entry = m_PageReturnHistory.Last.Value;
            if (entry.entityBase.destroyOnClose) {
                Game.Entity.DestroyEntity(entry.entityBase.EntityId);
                entry.entityBase = null;
                m_PageReturnHistory.Last.Value = entry;
            }
            else {
                entry.entityBase.Hide();
            }
        }

        public uint OpenPop(string address) {
            var popEntity = OpenPop<UIPopEntityBase>(address);
            if (popEntity == null) {
                return 0;
            }

            return popEntity.EntityId;
        }

        public T OpenPop<T>(string address) where T : UIPopEntityBase {
            if (!m_PopCanvas || string.IsNullOrEmpty(address)) {
                return null;
            }

            var popEntity = InstantiateUILayoutEntity<T>(address, m_PopCanvas);
            if (popEntity == null) {
                Debug.LogError($"[UI] Pop UILayout Entity {address} not found or not a {typeof(T).Name}!");
                return null;
            }

            m_PopEntities[popEntity.EntityId] = popEntity;
            return popEntity;
        }

        public void ClosePop(uint popEntityId) {
            if (popEntityId == 0) {
                return;
            }

            if (m_PopEntities.TryGetValue(popEntityId, out var popEntity)) {
                Game.Entity.DestroyEntity(popEntity.EntityId);
                m_PopEntities.Remove(popEntityId);
            }
        }

        public uint OpenOverlay(string address) {
            var overlayEntity = OpenOverlay<UIOverlayEntityBase>(address);
            if (overlayEntity == null) {
                return 0;
            }

            return overlayEntity.EntityId;
        }

        public T OpenOverlay<T>(string address) where T : UIOverlayEntityBase {
            if (!m_OverlayCanvas || string.IsNullOrEmpty(address)) {
                return null;
            }

            var overlayEntity = InstantiateUILayoutEntity<T>(address, m_OverlayCanvas);
            if (overlayEntity == null) {
                Debug.LogError($"[UI] Overlay UILayout Entity {address} not found or not a {typeof(T).Name}!");
                return null;
            }

            m_OverlayEntities[overlayEntity.EntityId] = overlayEntity;
            overlayEntity.gameObject.SetActive(true);
            return overlayEntity;
        }

        public void CloseOverlay(uint overlayEntityId) {
            if (overlayEntityId == 0) {
                return;
            }

            if (m_OverlayEntities.TryGetValue(overlayEntityId, out var overlayEntity)) {
                Game.Entity.DestroyEntity(overlayEntity.EntityId);
                m_OverlayEntities.Remove(overlayEntityId);
            }
        }

        protected override void OnInstantiated() {
            Game.UI = this;

            m_PageCanvas = CreateCanvas("PageCanvas", 0);
            m_PopCanvas = CreateCanvas("PopCanvas", 1);
            m_OverlayCanvas = CreateCanvas("OverlayCanvas", 2);
        }

        protected override void OnDestroyed() {
            m_PopEntities.Clear();
            m_OverlayEntities.Clear();
            m_PageReturnHistory.Clear();
        }

        private Canvas CreateCanvas(string canvasName, int sortingOrder) {
            var go = new GameObject(canvasName);
            go.transform.SetParent(transform, false);
            var canvas = go.AddComponent<Canvas>();
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.referencePixelsPerUnit = referencePixelsPerUnit;
            go.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            canvas.planeDistance = 1f;
            canvas.pixelPerfect = true;

            return canvas;
        }

        private T InstantiateUILayoutEntity<T>(string address, Canvas target) where T : UILayoutEntityBase {
            return Game.Entity.InstantiateEntity<T>(address, onInstantiatedCallback: entity => {
                entity.transform.SetParent(target.transform, false);
                var rectTransform = entity.GetComponent<RectTransform>();
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localScale = Vector3.one;
                entity.gameObject.SetActive(true);
            });
        }

        [Serializable]
        public struct PageReturnHistoryInfo {
            public string address;

            [FormerlySerializedAs("entity")]
            public UIPageEntityBase entityBase;

            public PageReturnHistoryInfo(string address, UIPageEntityBase entityBase) {
                this.address = address;
                this.entityBase = entityBase;
            }
        }
    }
}