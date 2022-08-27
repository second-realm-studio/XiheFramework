using System.Collections.Generic;
using UnityEngine;


namespace XiheFramework {
    /// <summary>
    /// UNDER DEVELOPMENT
    /// SWITCHING TO MIRROR
    /// </summary>
    public class NetworkModule : GameModule {
        public string gameVersion;
        public string nickName;
        public int maxPlayer=4;
        
        
        
        public void CreateRoom() {
            
        }
        
        private void Start() {
            
        }

        public override void Setup() {
            
        }

        public override void Update() {
        }

        public override void ShutDown(ShutDownType shutDownType) {
        }

        private void OnCreateRoom(object sender, object e) {
            
        }
        
        // #region PUN2 Callbacks
        //
        // public void OnConnected() {
        // }
        //
        // public void OnConnectedToMaster() {
        //     Debug.Log("Connected to Master");
        // }
        //
        // public void OnDisconnected(DisconnectCause cause) {
        // }
        //
        // public void OnRegionListReceived(RegionHandler regionHandler) {
        // }
        //
        // public void OnCustomAuthenticationResponse(Dictionary<string, object> data) {
        // }
        //
        // public void OnCustomAuthenticationFailed(string debugMessage) {
        // }
        //
        // #endregion
    }
}