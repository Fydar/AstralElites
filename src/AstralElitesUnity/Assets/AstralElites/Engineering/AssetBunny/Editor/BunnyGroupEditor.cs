#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    private static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/StreamingAssets/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                        BuildAssetBundleOptions.None,
                                        BuildTarget.WebGL);
    }
}

[CustomEditor(typeof(BunnyGroup), true)]
public class BunnyGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var assets = new HashSet<UnityEngine.Object>();

        var bunnyGroup = (BunnyGroup)target;

        var foldersToSearch = new HashSet<string>();

        foreach (string inclusion in bunnyGroup.InclusionLabels)
        {
            string[] foundAssetGuids = AssetDatabase.FindAssets($"l:{bunnyGroup.InclusionLabels}");

            foreach (string foundAssetGuid in foundAssetGuids)
            {
                string foundAssetPath = AssetDatabase.GUIDToAssetPath(foundAssetGuid);

                Debug.Log(foundAssetPath);
                if (AssetDatabase.IsValidFolder(foundAssetPath))
                {
                    foldersToSearch.Add(foundAssetPath);
                }
                else
                {
                    var mainAsset = AssetDatabase.LoadMainAssetAtPath(foundAssetPath);
                    assets.Add(mainAsset);
                }
            }
        }

        if (foldersToSearch.Count > 0)
        {
            string[] allFolderContents = AssetDatabase.FindAssets("t:Object", foldersToSearch.ToArray());

            foreach (string allFolderContent in allFolderContents)
            {
                string foundAssetPath = AssetDatabase.GUIDToAssetPath(allFolderContent);

                var mainAsset = AssetDatabase.LoadMainAssetAtPath(foundAssetPath);
                assets.Add(mainAsset);
            }
        }

        foreach (var asset in assets)
        {
            EditorGUILayout.LabelField(asset.name);
        }

        if (GUILayout.Button("Build"))
        {

        }
    }
}
#endif
