using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Audio;
using XiheFramework.Modules.Base;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace XiheFramework.Modules.Audio {
    /// <summary>
    /// Use FMOD to play audio
    /// </summary>
    public class AudioModule : GameModule {
        private readonly Dictionary<string, EventInstance> m_CachedEventInstances = new Dictionary<string, EventInstance>();

        /// <summary>
        /// Use this method to play a sound
        /// </summary>
        /// <param name="path"> audio event reference path inside FMOD </param>
        /// <param name="worldPos"></param>
        /// <param name="volume"></param>
        /// <param name="loop"></param>
        public void Play(string path, Vector3 worldPos, float volume, bool loop = false) {
            if (!InitEvent(path)) {
                return;
            }

            var instance = m_CachedEventInstances[path];
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPos));
            instance.start();
        }

        /// <summary>
        /// Init event reference to cache and does not play it
        /// </summary>
        /// <param name="path"></param>
        /// <returns> return true when ready, false when error </returns>
        public bool InitEvent(string path) {
            if (string.IsNullOrEmpty(path)) {
                return false;
            }

            if (m_CachedEventInstances.ContainsKey(path)) {
                return true;
            }

            RuntimeManager.StudioSystem.getEvent(path, out var eventDescription);
            if (eventDescription.isValid()) {
                var instance = RuntimeManager.CreateInstance(path);
                m_CachedEventInstances.Add(path, instance);
                return true;
            }

            Debug.LogError($"Audio reference path {path} is not valid");
            return false;
        }

        /// <summary>
        /// Init a event instance(if not already existed) by path and set its parameter
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public void SetParameter(string path, string parameterName, float value) {
            if (!InitEvent(path)) {
                return;
            }

            m_CachedEventInstances[path].setParameterByName(parameterName, value);
        }

        /// <summary>
        /// Set audio pausing state by path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="paused"></param>
        public void SetPaused(string path, bool paused) {
            if (!InitEvent(path)) {
                return;
            }

            m_CachedEventInstances[path].setPaused(paused);
        }

        /// <summary>
        /// Set all audio pausing state
        /// </summary>
        /// <param name="paused"></param>
        public void SetPausedAll(bool paused) {
            foreach (var eventInstance in m_CachedEventInstances.Values) {
                eventInstance.setPaused(paused);
            }
        }

        /// <summary>
        /// Stop audio by path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="stopMode"></param>
        public void Stop(string path, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT) {
            if (!InitEvent(path)) {
                return;
            }

            var instance = m_CachedEventInstances[path];
            instance.stop(stopMode);
            instance.release();
        }

        /// <summary>
        /// Stop all audio
        /// </summary>
        /// <param name="stopMode"></param>
        public void StopAll(STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT) {
            foreach (var eventInstance in m_CachedEventInstances.Values) {
                eventInstance.stop(stopMode);
                eventInstance.release();
            }
        }

        internal override void ShutDown(ShutDownType shutDownType) {
            StopAll();
        }
    }
}