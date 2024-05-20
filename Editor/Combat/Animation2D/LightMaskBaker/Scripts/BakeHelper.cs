using UnityEditor;
using UnityEngine;

namespace XiheFramework.Editor.Combat.Animation2D.LightMaskBaker.Scripts {
    public static class BakeHelper {
        public static void BakeRawEdge(ComputeShader shader, Texture source, float alphaThreshold, int extendRadius, ComputeShader blurShader, int blurRadius, int blurIteration,
            ComputeShader stepShader, float stepThreshold,
            ref RenderTexture mask) {
            if (shader == null) {
                return;
            }

            if (source == null) {
                return;
            }

            if (mask) {
                mask.Release();
                mask = null;
            }

            //create rt
            var edgeTexture = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.R8);
            Debug.Log($"w:{source.width} h:{source.height}");
            edgeTexture.filterMode = FilterMode.Bilinear;
            edgeTexture.enableRandomWrite = true;
            edgeTexture.Create();

            var groupCountX = Mathf.CeilToInt(source.width / 8.0f);
            var groupCountY = Mathf.CeilToInt(source.height / 8.0f);

            int bakeEdgeHandle = shader.FindKernel("BakeExtendMask");
            shader.SetTexture(bakeEdgeHandle, "InputTexture", source);
            shader.SetFloat("AlphaThreshold", alphaThreshold);
            shader.SetInt("ExtendRadius", extendRadius);
            shader.SetTexture(bakeEdgeHandle, "OutputTexture", edgeTexture);
            shader.Dispatch(bakeEdgeHandle, groupCountX, groupCountY, 1);

            mask = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.R8);
            mask.filterMode = FilterMode.Bilinear;
            mask.enableRandomWrite = true;
            mask.Create();

            Graphics.Blit(edgeTexture, mask);

            for (int i = 0; i < blurIteration; i++) {
                int bakeEdgeToVertexHandle = blurShader.FindKernel("BoxBlur");
                blurShader.SetTexture(bakeEdgeToVertexHandle, "InputTexture", edgeTexture);
                blurShader.SetInt("BlurRadius", blurRadius);
                blurShader.SetTexture(bakeEdgeToVertexHandle, "OutputTexture", mask);
                blurShader.Dispatch(bakeEdgeToVertexHandle, groupCountX, groupCountY, 1);

                //blit mask to edgetexture
                Graphics.Blit(mask, edgeTexture);
            }

            int stepHandle = stepShader.FindKernel("Step");
            stepShader.SetTexture(stepHandle, "InputTexture", edgeTexture);
            stepShader.SetFloat("Threshold", stepThreshold);
            stepShader.SetTexture(stepHandle, "OutputTexture", mask);
            stepShader.Dispatch(stepHandle, groupCountX, groupCountY, 1);

            RenderTexture.active = null;
            edgeTexture.Release();
        }

#if UNITY_EDITOR
        public static void SaveTexture(Texture2D source, Texture resultTexture, string suffix, TextureHelper.SaveTextureFileFormat fileFormat) {
            if (!resultTexture) {
                Debug.LogError($"Can't save empty {suffix} texture!");
                return;
            }

            // Save the result texture
            string path = AssetDatabase.GetAssetPath(source);
            string directory = System.IO.Path.GetDirectoryName(path);
            string filenameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(path);
            if (directory != null) {
                var extension = fileFormat.ToString().ToLower();
                string outputPath = System.IO.Path.Combine(directory, $"{filenameWithoutExtension}_{suffix}.{extension}");
                TextureHelper.SaveTextureToFile(resultTexture, outputPath, resultTexture.width, resultTexture.height, fileFormat);
            }

            AssetDatabase.Refresh();
        }
#endif
    }
}