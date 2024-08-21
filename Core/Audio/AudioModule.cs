using System.Collections.Generic;
using UnityEngine;
using XiheFramework.Core.Base;
using XiheFramework.Runtime;

namespace XiheFramework.Core.Audio {
    public class AudioModule : GameModule {
        private List<uint> m_PlayingEvents = new();

#if USE_WWISE
        public void Play(AK.Wwise.Event audioEvent, GameObject container = null, AkCallbackManager.EventCallback callback = null) {
            var id = audioEvent.Post(container, (uint)AkCallbackType.AK_EndOfEvent, callback);
            m_PlayingEvents.Add(id);
        }

        public void Stop(uint playingId, int fadeOutTime = 500, AkCurveInterpolation curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear) {
            AkSoundEngine.StopPlayingID(playingId, fadeOutTime, curveInterpolation);
        }

        public void SetVariable(string variableName, float value, GameObject target) {
            AkSoundEngine.SetRTPCValue(variableName, value, target);
        }

        public void SetVariableGlobal(string variableName, float value, GameObject target) {
            AkSoundEngine.SetRTPCValue(variableName, value, target);
        }

        public void SetState(string stateGroupName, string state) {
            AkSoundEngine.SetState(stateGroupName, state);
        }

#endif

        public void PlayViaUnity(uint ownerId, AudioClip audioClip, Vector3 worldPosition, bool loop) {
            var audioEntity = Game.Entity.InstantiateEntity<AudioEntity>("AudioEntity_StandardAudioEntity", ownerId, true, 0u, null);
            audioEntity.transform.position = worldPosition;
            audioEntity.AudioClip = audioClip;
            audioEntity.Loop = loop;
            audioEntity.Play();
        }

        protected override void Awake() {
            base.Awake();

            Game.Audio = this;
        }
    }
}