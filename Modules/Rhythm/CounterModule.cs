using System.Collections.Generic;
using UnityEngine;

namespace XiheFramework {
    public class CounterModule : GameModule {
        public List<string> candidates;
        public int listLength;

        //bottle setting
        public int bottleCount; //same as depth 

        private Queue<string> m_WaitingList;

        private BottleData m_BottleData;

        private void Start() {
            //GenerateRandomList();
            //m_Inputs = new Queue<char>();

            m_BottleData = new BottleData(bottleCount);
            m_BottleData.autoClearBottle = false;

            Game.Event.Subscribe("OnPlay", OnPlay);
            Game.Event.Subscribe("OnPotionCreated", OnPotionCreated);
            Game.Event.Subscribe("OnBeatInput", OnBeatInput);
            //Game.Event.Subscribe("OnBeat",OnBeat);
        }

        private void OnBeat(object sender, object e) {
            //m_BottleData.Shift();
        }

        private void OnBeatInput(object sender, object e) {
            var ne = (int) e;

            m_BottleData.Distribute(ne);
            EstimateFullBottle();
            m_BottleData.Shift();
            
            Game.Event.Invoke("OnBottleDataUpdated", this, m_BottleData);
        }

        private void EstimateFullBottle() {
            var full = m_BottleData.Peek();
            if (full.Length < m_BottleData.Count()) {
                return;
            }

            var waiting = m_WaitingList.Peek();
            var bottle = m_BottleData.Peek();
            for (int i = 0; i < m_BottleData.Count(); i++) {
                if (waiting[i] != bottle[i]) {
                    //not correct
                    Game.Event.Invoke("OnPotionFinished", this, false);
                    return;
                }
            }

            Game.Event.Invoke("OnPotionFinished", this, true);
        }

        private void OnPlay(object sender, object e) {
            GenerateRandomList();
        }

        void GenerateRandomList() {
            m_WaitingList = new Queue<string>();
            for (int i = 0; i < listLength; i++) {
                var ran = Random.Range(0, candidates.Count);
                m_WaitingList.Enqueue(candidates[ran]);
            }

            Game.Event.Invoke("OnWaitingListUpdated", this, m_WaitingList.ToArray());
        }

        private void OnPotionCreated(object sender, object e) {
            var ne = (string) e;

            if (string.Equals(m_WaitingList.Peek(), ne)) {
                m_WaitingList.Dequeue();
                Game.Event.Invoke("OnWaitingListUpdated", this, m_WaitingList.ToArray());
            }
        }

        public override void Update() { }

        public override void ShutDown(ShutDownType shutDownType) {
            
        }
    }
}