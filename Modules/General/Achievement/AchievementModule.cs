using System.Collections.Generic;
using System.Linq;

namespace XiheFramework {
    public class AchievementModule : GameModule {
        public List<AchievementData> achievements;
        private Dictionary<int, AchievementData> m_Achievements = new Dictionary<int, AchievementData>();

        private Dictionary<int, AchievementProgress> m_Progresses = new Dictionary<int, AchievementProgress>();

        public AchievementData GetAchievementData(int id) {
            if (m_Achievements.ContainsKey(id)) {
                return m_Achievements[id];
            }

            return null;
        }

        public int GetAchievementProgress(int id) {
            if (m_Progresses.ContainsKey(id)) {
                return m_Progresses[id].progress;
            }

            return -1;
        }

        public bool IsAchieved(int id) {
            if (m_Achievements.ContainsKey(id)) {
                return m_Progresses[id].progress >= m_Achievements[id].progressionLimit;
            }

            return false;
        }

        public override void Setup() {
            base.Setup();

            foreach (var achievement in achievements.Where(achievement => achievement != null)) {
                if (m_Achievements.ContainsKey(achievement.id)) {
                    m_Achievements[achievement.id] = achievement;
                }
                else {
                    m_Achievements.Add(achievement.id, achievement);
                }

                //initialize progress
                if (m_Progresses.ContainsKey(achievement.id)) {
                    m_Progresses[achievement.id].progress = 0;
                }
                else {
                    m_Progresses.Add(achievement.id, new AchievementProgress(achievement.id, 0));
                }
                
                //override progress from save data
                
            }
        }

        public override void Update() {
            RunAchievementLogic();
        }


        public override void ShutDown(ShutDownType shutDownType) {
        }

        private void RunAchievementLogic() {
        }
    }
}