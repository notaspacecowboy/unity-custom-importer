using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EditorUtilities 
{
    public static void ExtractMaterials(string assetPath, string destinationPath)
    {
        var importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        if (importer == null)
            return;

        var assetsToReload = new HashSet<string>();

        var materials = AssetDatabase.LoadAllAssetsAtPath(importer.assetPath).Where(x => x.GetType() == typeof(Material)).ToArray();

        foreach (var material in materials)
        {
            var newAssetPath = CombinePaths(destinationPath, material.name) + ".mat";
            newAssetPath = AssetDatabase.GenerateUniqueAssetPath(newAssetPath);

            var error = AssetDatabase.ExtractAsset(material, newAssetPath);
            if (String.IsNullOrEmpty(error))
            {
                assetsToReload.Add(importer.assetPath);
            }
        }

        foreach (var path in assetsToReload)
        {
            AssetDatabase.WriteImportSettingsIfDirty(path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
    }

    public static string CombinePaths(params string[] paths)
    {
        if (null == paths)
            return string.Empty;
        return string.Join('/', paths);
    }
}
