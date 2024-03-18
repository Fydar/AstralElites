using System.Reflection;
using System;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
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

        foreach (var inclusion in bunnyGroup.InclusionLabels)
        {
            var foundAssetGuids = AssetDatabase.FindAssets($"l:{bunnyGroup.InclusionLabels}");

            foreach (var foundAssetGuid in foundAssetGuids)
            {
                var foundAssetPath = AssetDatabase.GUIDToAssetPath(foundAssetGuid);

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
            var allFolderContents = AssetDatabase.FindAssets("t:Object", foldersToSearch.ToArray());

            foreach (var allFolderContent in allFolderContents)
            {
                var foundAssetPath = AssetDatabase.GUIDToAssetPath(allFolderContent);

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
