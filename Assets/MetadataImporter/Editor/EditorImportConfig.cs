using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorImportConfig
{
    private const string KDefaultAssetName = "DefaultAsset";
    public string TemplateName { get; set; }
    public string ResourcePath { get; set; }
    public string AssetPath { get; set; }
    public string AssetName { get; set; }

    public bool Validated { get; set; }
    public string ErrorMessage { get; set; }

    public string AlbedoMapPath { get; set; }

    public string NormalMapPath { get; set; }
    public string MetallicMapPath { get; set; }
    public string RoughnessMapPath { get; set; }
    public string HeightMapPath { get; set; }
    public string OcclusionMapPath { get; set; }
    public string EmissionMapPath { get; set; }
    public string DetailMaskPath { get; set; }
    public string SpecularMapPath { get; set; }
    public bool ParadataOnly { get; set; }

    public void Reset()
    {
        ResourcePath = Application.dataPath;
        AssetPath = Application.dataPath;
        AssetName = KDefaultAssetName;
        Validated = false;
        ErrorMessage = "";
        ParadataOnly = false;
    }
}