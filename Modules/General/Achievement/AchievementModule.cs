using System.Collections.Generic;
using System.Linq;

namespace XiheFramework {
    public class AchievementModule : GameModule {
        public List<AchievementData> achievements;
        private Dictionary<string, AchievementData> m_Achievements = new Dictionary<string, AchievementData>();

        private Dictionary<string, AchievementProgress> m_Progresses = new Dictionary<string, AchievementProgress>();

        public override void Setup() {
            base.Setup();

            foreach (var achievement in achievements.Where(achievement => achievement != null)) {
                if (m_Achievements.ContainsKey(achievement.achievementName)) {
                    m_Achievements[achievement.achievementName] = achievement;
                }
                else {
                    m_Achievements.Add(achievement.achievementName, achievement);
                }
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