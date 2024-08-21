using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace XiheFramework.Core.Base
{
    /// <summary>
    /// 必须挂在存在collider才能生效
    /// 方便一个控件控制多个可点击的collider
    /// </summary>
    public class ClickableMono : MonoBehaviour,IPointerClickHandler
    {
        /// <summary>
        /// 被点击后触发的动作
        /// </summary>
        private Action m_ClickAction;

        /// <summary>
        /// 注册动作
        /// </summary>
        /// <param name="clickAction"></param>
        public void Init(Action clickAction)
        {
            m_ClickAction = clickAction;
        }

        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if(m_ClickAction == null)
                return;
            m_ClickAction.Invoke();
        }
    }
}
