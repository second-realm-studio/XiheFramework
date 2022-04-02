using System.Linq;
using UnityEngine;

namespace XiheFramework {
    public abstract class UIBehaviour : MonoBehaviour
    {
        public string uiName;
        public virtual void Start()
        {
            Register();
        }

        public virtual void Register()
        {
            if (uiName == string.Empty)
            {
                uiName = gameObject.name + gameObject.GetHashCode();
            }
            Game.UI.RegisterUIBehaviour(uiName, this);
        }
        public virtual void Active() {
            gameObject.SetActive(true);
        }

        public virtual void UnActive() {
            gameObject.SetActive(false);
        }
    }
}