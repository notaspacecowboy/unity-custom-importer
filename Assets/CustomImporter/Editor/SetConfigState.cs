using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class ImportConfig
{
    private const string KDefaultAssetName = "DefaultAsset";

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

    public void Reset()
    {
        ResourcePath = Application.dataPath;
        AssetPath = Application.dataPath;
        AssetName = KDefaultAssetName;
        Validated = false;
        ErrorMessage = "";
    }
}

public class SetConfigState : IImportWindowState
{
    #region string literals

    private const string KResourceNotFoundErr = "Resource file does not exist!";
    private const string KAssetPathNotExistErr = "Resource file does not exist!";
    private const string KEmptyAssetNameErr = "Resource file does not exist!";

    #endregion

    private ImportConfig _mImportConfig = new ImportConfig();

    public SetConfigState(string resourcePath, EditorWindow window, StateMachine owner) : base(window, owner)
    {
        _mImportConfig.Reset();
        _mImportConfig.ResourcePath = resourcePath;
    }

    public override void Update()
    {
        GUILayout.Space(20);

        GUILayout.Label("Model & Material Configuration", EditorStylesHelper.TitleStyle);

        GUILayout.Space(20);

        //resource path
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Model Path: ", EditorStylesHelper.LabelStyle, GUILayout.Width(120));
        EditorGUILayout.LabelField(_mImportConfig.ResourcePath, GUILayout.Width(400));
        GUILayout.Space(20);
        if (GUILayout.Button("...", GUILayout.Width(25)))
        {
            _mImportConfig.ResourcePath = EditorUtility.OpenFilePanel("Choose Resource File", ".", "fbx");
            EditorWindow.Repaint();
        }
        GUILayout.EndHorizontal();

        //asset path
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Asset Path: ", EditorStylesHelper.LabelStyle, GUILayout.Width(120));
        EditorGUILayout.LabelField(_mImportConfig.AssetPath, GUILayout.Width(400));
        GUILayout.Space(20);
        if (GUILayout.Button("...", GUILayout.Width(25)))
        {
            _mImportConfig.AssetPath = EditorUtility.OpenFolderPanel("Choose Model Directory", ".", "");
            EditorWindow.Repaint();
        }
        GUILayout.EndHorizontal();

        //asset name
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Asset Name: ", EditorStylesHelper.LabelStyle, GUILayout.Width(120));
        _mImportConfig.AssetName = EditorGUILayout.TextField(_mImportConfig.AssetName, GUILayout.Width(300));
        GUILayout.EndHorizontal();

        //albedo map path
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Albedo Map Path: ", EditorStylesHelper.LabelStyle, GUILayout.Width(120));
        EditorGUILayout.LabelField(_mImportConfig.AlbedoMapPath, GUILayout.Width(400));
        GUILayout.Space(20);
        if (GUILayout.Button("...", GUILayout.Width(25)))
        {
            _mImportConfig.AlbedoMapPath = EditorUtility.OpenFilePanel("Choose Resource File", ".", "jpg,jpeg,png");
            EditorWindow.Repaint();
        }
        GUILayout.EndHorizontal();

        //normal map path
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Normal Map Path: ", EditorStylesHelper.LabelStyle, GUILayout.Width(120));
        EditorGUILayout.LabelField(_mImportConfig.NormalMapPath, GUILayout.Width(400));
        GUILayout.Space(20);
        if (GUILayout.Button("...", GUILayout.Width(25)))
        {
            _mImportConfig.NormalMapPath = EditorUtility.OpenFilePanel("Choose Resource File", ".", "jpg,jpeg,png");
            EditorWindow.Repaint();
        }
        GUILayout.EndHorizontal();
        

        GUILayout.Space(30);


        //back button
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        if (GUILayout.Button("Back", EditorStylesHelper.RegularButtonStyle, GUILayout.Height(20), GUILayout.Width(535)))
        {
            ChangeState(new SelectModelState(EditorWindow, Owner));
        }
        EditorGUILayout.EndHorizontal();

        //validate button
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        if (GUILayout.Button("Validate", EditorStylesHelper.WarnButtonStyle, GUILayout.Height(20), GUILayout.Width(535)))
        {
            if (!System.IO.File.Exists(_mImportConfig.ResourcePath))
                _mImportConfig.ErrorMessage = KResourceNotFoundErr;
            else if (!System.IO.Directory.Exists(_mImportConfig.AssetPath))
                _mImportConfig.ErrorMessage = KAssetPathNotExistErr;
            else if (string.IsNullOrEmpty(_mImportConfig.AssetName))
                _mImportConfig.ErrorMessage = KEmptyAssetNameErr;
            else
            {
                _mImportConfig.Validated = true;
                _mImportConfig.ErrorMessage = "";
            }
        }
        EditorGUILayout.EndHorizontal();

        //error message
        if (!_mImportConfig.Validated && _mImportConfig.ErrorMessage != "")
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(_mImportConfig.ErrorMessage, EditorStylesHelper.ErrorLabelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        //success message
        if (_mImportConfig.Validated)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Setup has been validated!", EditorStylesHelper.SuccessLabelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        //create button
        if (_mImportConfig.Validated)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if (GUILayout.Button("Create", EditorStylesHelper.RegularButtonStyle, GUILayout.Height(20), GUILayout.Width(535)))
            {
                var state = new SetupMetadataState(_mImportConfig, EditorWindow, Owner);
                ChangeState(state);
            }
            EditorGUILayout.EndHorizontal();

        }

        EditorWindow.minSize = EditorWindow.maxSize = new Vector2(600, 300);
    }
}
