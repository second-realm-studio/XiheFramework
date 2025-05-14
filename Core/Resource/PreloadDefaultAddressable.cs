using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Resource {
    public class PreloadDefaultAddressable : MonoBehaviour {
#if USE_TMP
        public TMP_Text display;
#else
        public Text display;
#endif
        public string[] labels = { "default" };

        private void Start() {
            StartCoroutine(LoadLabelAsync(labels));
        }

        IEnumerator LoadLabelAsync(IEnumerable<string> label) {
            var handle = Game.Resource.LoadAssetsAsyncCoroutine(label, onLoaded: (address) => {
                if (display != null) display.text = $"loaded: {address}";
            });
            yield return handle;

            Game.Event.Invoke(ResourceModuleEvents.OnDefaultResourcesLoadedEvtName);
        }
    }
}