#if UNITY_EDITOR && UNITY_WEBGL
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BuildStripper : IPostprocessBuildWithReport, IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        PostProcessData();
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        PostProcessData();
    }

    private void PostProcessData()
    {
        var rendererDataList = (ScriptableRendererData[])typeof(UniversalRenderPipelineAsset).GetField("m_RendererDataList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(UniversalRenderPipeline.asset);

        for (int i = 0; i < rendererDataList.Length; i++)
        {
            var rendererData = (Renderer2DData)rendererDataList[i];
            if (rendererData != null)
            {
                var postProcessingData = (PostProcessData)typeof(Renderer2DData).GetField("m_PostProcessData", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(rendererData);

                foreach (var texture in postProcessingData.textures.blueNoise16LTex)
                {
                    RemoveFromBuild(texture);
                }
                foreach (var texture in postProcessingData.textures.filmGrainTex)
                {
                    RemoveFromBuild(texture);
                }
                RemoveFromBuild(postProcessingData.textures.smaaAreaTex);
                RemoveFromBuild(postProcessingData.textures.smaaSearchTex);
            }
        }

        foreach (var texture in Resources.FindObjectsOfTypeAll(typeof(Texture)))
        {
            if (texture.name == "Splash Screen Unity Logo")
            {
                // RemoveFromBuild(texture);
            }
            if (texture.name == "FalloffLookupTexture")
            {
                RemoveFromBuild(texture);
            }
        }

        static void RemoveFromBuild(Object texture)
        {
            texture.hideFlags |= HideFlags.DontSaveInBuild;

            if (texture is Texture2D texture2D)
            {
                texture2D.Reinitialize(2, 2);
                texture2D.SetPixels32(new Color32[]
                {
                    Color.clear,
                    Color.clear,
                    Color.clear,
                    Color.clear
                });
                texture2D.Apply();
            }

            EditorUtility.SetDirty(texture);
        }
    }
}
#endif
