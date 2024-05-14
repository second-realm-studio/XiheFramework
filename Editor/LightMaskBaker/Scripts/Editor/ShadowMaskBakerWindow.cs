using UnityEditor;
using UnityEngine;

namespace XiheFramework.Combat.Animation2D.LightMaskBaker.Scripts.Editor {
    public class ShadowMaskBakerWindow : EditorWindow {
        private Texture2D m_SourceTexture;
        private ComputeShader m_ExtendShader;
        private ComputeShader m_BlurShader;
        private ComputeShader m_StepShader;

        private float m_TextureFieldWidth = 100f;
        private bool m_IsBakeRawEdgeRealtime = false;
        private float m_EdgeAlphaThreshold = 0.5f;
        private int m_ExtendWidth = 10;
        private int m_BlurRadius = 2;
        private int m_BlurIteration = 10;

        private float m_BlurStepThreshold = 0.5f;
        private TextureHelper.SaveTextureFileFormat m_SaveTextureFileFormat = TextureHelper.SaveTextureFileFormat.PNG;

        private RenderTexture m_ResultTexture;
        private RenderTexture m_Outline;

        [MenuItem("Xihe/Shadow Mask Baker")]
        public static void ShowWindow() {
            GetWindow<ShadowMaskBakerWindow>("Outline Shadow Baker");
        }

        private void OnEnable() {
            var bakeRawShaderGuid = AssetDatabase.FindAssets("ExtendPixel t:ComputeShader");
            var bakeVertexShaderGuid = AssetDatabase.FindAssets("BoxBlur t:ComputeShader");
            var stepShaderGuid = AssetDatabase.FindAssets("Step t:ComputeShader");
            // var bakeOutlineMaskShaderGuid = AssetDatabase.FindAssets("BakeOutlineMask t:ComputeShader");

            if (bakeRawShaderGuid.Length > 0) {
                m_ExtendShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(AssetDatabase.GUIDToAssetPath(bakeRawShaderGuid[0]));
            }

            if (bakeVertexShaderGuid.Length > 0) {
                m_BlurShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(AssetDatabase.GUIDToAssetPath(bakeVertexShaderGuid[0]));
            }

            if (stepShaderGuid.Length > 0) {
                m_StepShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(AssetDatabase.GUIDToAssetPath(stepShaderGuid[0]));
            }

            // if (bakeOutlineMaskShaderGuid.Length > 0) {
            //     m_BakeOutlineMaskShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(AssetDatabase.GUIDToAssetPath(bakeOutlineMaskShaderGuid[0]));
            // }
        }

        private void OnGUI() {
            #region Title

            //title
            GUILayout.Label("Sprite SDF-based Normal Generator ", EditorStyles.boldLabel);

            #endregion

            EditorGUILayout.Space();

            #region Texture

            var width = m_TextureFieldWidth;
            var height = m_SourceTexture ? m_TextureFieldWidth * m_SourceTexture.height / m_SourceTexture.width : m_TextureFieldWidth;
            //source texture
            GUILayout.BeginHorizontal();
            m_SourceTexture = GUIHelper.TextureField("Source", m_SourceTexture, width, height);
            //result textures
            GUILayout.Label("Results:", EditorStyles.boldLabel);
            GUIHelper.DisplayTextureBox("Raw Edge", m_ResultTexture, width, height);
            GUILayout.EndHorizontal();

            #endregion

            m_TextureFieldWidth = EditorGUILayout.Slider("Texture Display Width", m_TextureFieldWidth, 1, 200);

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            #region Bake Raw Edge

            //compute edge
            GUILayout.Label("1. Bake Raw Edge", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Extend Shader", EditorStyles.boldLabel);
            m_ExtendShader = (ComputeShader)EditorGUILayout.ObjectField(m_ExtendShader, typeof(ComputeShader), false);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Blur Shader", EditorStyles.boldLabel);
            m_BlurShader = (ComputeShader)EditorGUILayout.ObjectField(m_BlurShader, typeof(ComputeShader), false);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Step Shader", EditorStyles.boldLabel);
            m_StepShader = (ComputeShader)EditorGUILayout.ObjectField(m_StepShader, typeof(ComputeShader), false);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Alpha Ignore Threshold", EditorStyles.boldLabel);
            m_EdgeAlphaThreshold = EditorGUILayout.Slider(m_EdgeAlphaThreshold, 0, 1);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Extend Pixel Radius", EditorStyles.boldLabel);
            m_ExtendWidth = EditorGUILayout.IntSlider(m_ExtendWidth, 0, 50);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Blur Radius", EditorStyles.boldLabel);
            m_BlurRadius = EditorGUILayout.IntSlider(m_BlurRadius, 0, 20);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Blur Iteration", EditorStyles.boldLabel);
            m_BlurIteration = EditorGUILayout.IntSlider(m_BlurIteration, 0, 50);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Blur Threshold", EditorStyles.boldLabel);
            m_BlurStepThreshold = EditorGUILayout.Slider(m_BlurStepThreshold, 0, 1f);
            GUILayout.EndHorizontal();


            m_IsBakeRawEdgeRealtime = GUILayout.Toggle(m_IsBakeRawEdgeRealtime, "Refresh Raw Edge Realtime");
            if (m_IsBakeRawEdgeRealtime) {
                BakeHelper.BakeRawEdge(m_ExtendShader, m_SourceTexture, m_EdgeAlphaThreshold, m_ExtendWidth, m_BlurShader, m_BlurRadius, m_BlurIteration,
                    m_StepShader, m_BlurStepThreshold,
                    ref m_ResultTexture);
            }
            else {
                if (GUILayout.Button("Bake")) {
                    BakeHelper.BakeRawEdge(m_ExtendShader, m_SourceTexture, m_EdgeAlphaThreshold, m_ExtendWidth, m_BlurShader, m_BlurRadius, m_BlurIteration,
                        m_StepShader, m_BlurStepThreshold, ref m_ResultTexture);
                }
            }

            #endregion

            #region Save/Clear

            GUILayout.BeginHorizontal();
            GUILayout.Label("Save as: ", EditorStyles.boldLabel);
            m_SaveTextureFileFormat = (TextureHelper.SaveTextureFileFormat)EditorGUILayout.EnumPopup(m_SaveTextureFileFormat);
            GUILayout.EndHorizontal();

            //clear/save
            if (GUILayout.Button("Save")) {
                BakeHelper.SaveTexture(m_SourceTexture, m_ResultTexture, "ShadowMask", m_SaveTextureFileFormat);
            }

            if (GUILayout.Button("Clear Cache")) {
                TextureHelper.ClearTempTextures(m_ResultTexture);
                m_ResultTexture = null;

                // m_VertexData = null;
            }

            #endregion
        }
    }
}