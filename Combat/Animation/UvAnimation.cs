using UnityEngine;

namespace XiheFramework.Combat.Animation {
    [CreateAssetMenu(fileName = "New FlipBook Animation", menuName = "Terra/Animation/FlipBook Animation", order = 1)]
    public class UvAnimation : ScriptableObject {
        public Texture2D texture;
        public Texture2D shadowTexture;
        public Texture2D colorRampTexture;

        public int columns;
        public int rows;
        public int totalFrames;

        //fine tuning
        public float shadowThickness = 0.015f;
        public Vector2 shadowOffsetMin = new(-0.01f, -0.01f);
        public Vector2 shadowOffsetMax = new(0.01f, 0.01f);
        public Color shadowColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    }
}