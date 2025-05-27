using System;
using XiheFramework.Runtime.Base;

namespace XiheFramework.Runtime.Serialization {
    public abstract class SerializationModuleBase : GameModuleBase, ISerializationModule {
        public string OnSaveEventName => "Serialization.OnSave";
        public string OnLoadEventName => "Serialization.OnLoad";

        public void SaveGame() {
            OnSaveCallback();

            var args = new OnSaveEventArgs();
            args.timeStamp = DateTime.Now;
            Game.Event.InvokeNow(OnSaveEventName, null, args);
        }

        public void LoadGame() {
            OnLoadCallback();

            var args = new OnLoadEventArgs();
            Game.Event.InvokeNow(OnLoadEventName, null, args);
        }

        protected override void OnInstantiated() {
            Game.Serialization = this;
        }

        protected virtual void OnSaveCallback() { }
        protected virtual void OnLoadCallback() { }
    }
}