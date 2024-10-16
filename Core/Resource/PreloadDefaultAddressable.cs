using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Resource {
    public class PreloadDefaultAddressable : MonoBehaviour {
        public Text display;
        public string[] labels = { "default" };

        private void Start() {
            StartCoroutine(LoadLabelAsync(labels));
        }

        IEnumerator LoadLabelAsync(IEnumerable<string> label) {
            var handle = Game.Resource.LoadAssetsAsyncCoroutine(label, onLoaded: (address) => {
                if (display != null) display.text = $"loaded: {address}";
            });
            yield return handle;

            Game.Event.Invoke(Game.Resource.onDefaultResourcesLoadedEvtName);
        }
    }
}