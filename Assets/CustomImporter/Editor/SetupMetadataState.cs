using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;
using static UnityEngine.GraphicsBuffer;

public class SetupMetadataState : IImportWindowState
{
    private static readonly string kAlbedoMapName = "albedo";
    private static readonly string kNormalMapName = "normal";

    private ImportConfig _mImportConfig;

    private ModelRef _mModelRef;

    private Vector2 _mScrollPosition;

    private ModelData _mParentModelData;

    public SetupMetadataState(ImportConfig config, EditorWindow window, StateMachine owner) : base(window, owner)
    {
        _mImportConfig = config;
        LoadModelAndExtractMaterial().Forget();
    }

    public SetupMetadataState(ModelRef existingModelRef, EditorWindow window, StateMachine owner) : base(window, owner)
    {
        _mModelRef = existingModelRef;

        string path = AssetDatabase.GetAssetPath(_mModelRef);
        if(path == null)
            Debug.LogError("failed to find metadata file");
        _mImportConfig = new ImportConfig();
        _mImportConfig.AssetPath = Path.GetDirectoryName(path);
        _mImportConfig.AssetName = Path.GetFileNameWithoutExtension(path);
        Debug.Log("asset path: " + _mImportConfig.AssetPath);
        Debug.Log("asset name: " + _mImportConfig.AssetName);
    }

    public override void Update()
    {
        if (_mModelRef == null)
            return;

        _mScrollPosition = EditorGUILayout.BeginScrollView(_mScrollPosition);

        GUILayout.Space(20);

        GUILayout.Label("Metadata Configuration", EditorStylesHelper.TitleStyle);

        GUILayout.Space(20);

        //at top level
        if (_mParentModelData == null)
        {
            ShowModelData(_mModelRef.Root);
        }
        else
        {
            foreach (var mChild in _mParentModelData.SubModels)
            {
                ShowModelData(mChild);
                GUILayout.Space(10);
            }
        }

        EditorGUILayout.EndScrollView();

        //back button
        GUILayout.Space(20);
        EditorStylesHelper.HorizontalCenteredButton(OnBackButtonClicked, "Back", EditorStylesHelper.WarnButtonStyle, GUILayout.Height(30), GUILayout.Width(400));
        EditorStylesHelper.HorizontalCenteredButton(OnSaveButtonClicked, "Save", EditorStylesHelper.RegularButtonStyle, GUILayout.Height(30), GUILayout.Width(400));
        GUILayout.Space(40);

        EditorWindow.minSize = EditorWindow.maxSize = new Vector2(600, 600);
    }

    private void ShowModelData(ModelData modelData)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUIStyle modelNameStyle = EditorStylesHelper.LabelStyle;
        modelNameStyle.normal.textColor = EditorStylesHelper.LightGreen;
        GUILayout.Label(">  " + modelData.Name, modelNameStyle);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);

        //name
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Name: ", EditorStylesHelper.LabelStyle);
        modelData.Name = EditorGUILayout.TextField(modelData.Name, GUILayout.Width(300));
        GUILayout.EndHorizontal();

        //description
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Description: ", EditorStylesHelper.LabelStyle);
        modelData.Description = EditorGUILayout.TextField(modelData.Description, GUILayout.Width(300));
        GUILayout.EndHorizontal();

        //URL
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("URL: ", EditorStylesHelper.LabelStyle);
        modelData.Url = EditorGUILayout.TextField(modelData.Url, GUILayout.Width(300));
        GUILayout.EndHorizontal();

        //image
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Image: ", EditorStylesHelper.LabelStyle);
        modelData.Image = (Image)EditorGUILayout.ObjectField(modelData.Image, typeof(Image), false, GUILayout.Width(300));
        GUILayout.EndHorizontal();

        //video clip
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Video: ", EditorStylesHelper.LabelStyle);
        modelData.Video = (VideoClip)EditorGUILayout.ObjectField(modelData.Video, typeof(VideoClip), false, GUILayout.Width(300));
        GUILayout.EndHorizontal();


        //sub model button
        if (modelData.Transform.childCount == 0)
            return;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Go to its sub-models? ", EditorStylesHelper.LabelStyle);
        if (GUILayout.Button("Go", EditorStylesHelper.RegularButtonStyle, GUILayout.Width(40)))
        {
            CreateModelDataForEachSubModel(modelData.Transform, modelData);
            _mParentModelData = modelData;
            EditorWindow.Repaint();
        }
        GUILayout.EndHorizontal();
    }

#region import model and create material
    private async UniTask LoadModelAndExtractMaterial()
    {
        //copy all resources from resource path to unity project
        CopyAllResources();

        await UniTask.DelayFrame(1);

        // Import the copied asset
        string absDstPath = _mImportConfig.AssetPath;
        string relDstPath = absDstPath.Replace(Application.dataPath, "Assets");
        string relModelPath = EditorUtilities.CombinePaths(relDstPath, _mImportConfig.AssetName) + ".FBX";
        AssetDatabase.ImportAsset(
            relModelPath,
            ImportAssetOptions.ForceUpdate);

        if (_mImportConfig.AlbedoMapPath != null && File.Exists(_mImportConfig.AlbedoMapPath))
        {
            string relAlbedoPath = EditorUtilities.CombinePaths(relDstPath, kAlbedoMapName) + "." + _mImportConfig.AlbedoMapPath.Split('.').Last();
            Debug.Log(relAlbedoPath);
            AssetDatabase.ImportAsset(
                relAlbedoPath,
                ImportAssetOptions.ForceUpdate);
        }

        if (_mImportConfig.NormalMapPath != null && File.Exists(_mImportConfig.NormalMapPath))
        {
            string relNormalPath = EditorUtilities.CombinePaths(relDstPath, kNormalMapName) + "." + _mImportConfig.NormalMapPath.Split('.').Last();
            Debug.Log(relNormalPath);
            AssetDatabase.ImportAsset(
                relNormalPath,
                ImportAssetOptions.ForceUpdate);
        }

        await UniTask.DelayFrame(1);


        EditorUtilities.ExtractMaterials(
            relModelPath,
            relDstPath);
        

        await UniTask.DelayFrame(1);

        await UniTask.DelayFrame(1);

        //load model
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(relModelPath);

        //create model ref
        _mModelRef = ScriptableObject.CreateInstance<ModelRef>();
        _mModelRef.Root = new ModelData();
        _mModelRef.Root.Transform = model.transform;
        _mModelRef.Model = model;
    }


    private void CopyAllResources()
    {
        File.Copy(
            _mImportConfig.ResourcePath,
            EditorUtilities.CombinePaths(_mImportConfig.AssetPath, _mImportConfig.AssetName) + ".FBX",
            true);

        if (_mImportConfig.AlbedoMapPath != null && File.Exists(_mImportConfig.AlbedoMapPath))
        {
            string albedoPath = EditorUtilities.CombinePaths(_mImportConfig.AssetPath, kAlbedoMapName) + "." + _mImportConfig.AlbedoMapPath.Split('.').Last();
            File.Copy(
                _mImportConfig.AlbedoMapPath,
                albedoPath,
                true);
        }

        if (_mImportConfig.NormalMapPath != null && File.Exists(_mImportConfig.NormalMapPath))
        {
            string normalPath = EditorUtilities.CombinePaths(_mImportConfig.AssetPath, kNormalMapName) + "." + _mImportConfig.NormalMapPath.Split('.').Last();
            File.Copy(
                _mImportConfig.NormalMapPath,
                normalPath,
                true);
        }
    }
    

#endregion

    private void CreateModelDataForEachSubModel(Transform tParent, ModelData mParent)
    {
        for (int i = 0; i < tParent.childCount; i++)
        {
            Transform tChild = tParent.GetChild(i);
            var mChild = new ModelData();
            mChild.Transform = tChild;
            mParent.SubModels.Add(mChild);
            mChild.Parent = mParent; 
        }
    }

    private void OnBackButtonClicked()
    {
        if (_mParentModelData == null)
        {
            SelectModelState state = new SelectModelState(EditorWindow, Owner);
            ChangeState(state);
        }
        else
        {
            _mParentModelData = _mParentModelData.Parent;
            EditorWindow.Repaint();
        }
    }


    private void OnSaveButtonClicked()
    {
        CalculateSizeAndCenter(_mModelRef.Root);

        string absDstPath = EditorUtilities.CombinePaths(_mImportConfig.AssetPath, _mImportConfig.AssetName) + ".asset";
        string relDstPath = absDstPath.Replace(Application.dataPath, "Assets");
        // If asset already exists at the path, delete it
        if (AssetDatabase.LoadAssetAtPath(relDstPath, typeof(ModelRef)) != null)
        {
            EditorUtility.SetDirty(_mModelRef);
            AssetDatabase.SaveAssets();
        }
        else
        {
            AssetDatabase.CreateAsset(_mModelRef, relDstPath);
            AssetDatabase.SaveAssets();
        }
    }

    private void CalculateSizeAndCenter(ModelData modelData)
    {
        DoCalculateSizeAndCenter(modelData.Transform.gameObject, modelData);
        foreach (var mChild in modelData.SubModels)
            CalculateSizeAndCenter(mChild);
    }

    private void DoCalculateSizeAndCenter(GameObject obj, ModelData modelData)
    {
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        foreach (Renderer r in obj.GetComponentsInChildren<Renderer>())
            bounds.Encapsulate(r.bounds);

        modelData.Size = bounds.size;
        modelData.Center = bounds.center;
    }
}
