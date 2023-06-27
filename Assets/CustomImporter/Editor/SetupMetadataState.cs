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

public class SetupMetadataState : IImportWindowState
{
    private static readonly string k_albedoMapName = "albedo";
    private static readonly string k_normalMapName = "normal";

    private ImportConfig m_importConfig;

    private ModelRef m_modelRef;

    private Vector2 m_scrollPosition;

    private ModelData m_parentModelData;

    private MetadataTemplate m_activeTemplate;
    
    public SetupMetadataState(ImportConfig config, EditorWindow window, StateMachine owner) : base(window, owner)
    {
        m_importConfig = config;
        LoadModelAndExtractMaterial().Forget();


        foreach (var template in TemplateImporter.Instance.Template.Templates)
        {
            if (template.Name == m_importConfig.TemplateName)
            {
                m_activeTemplate = template;
                break;
            }
        }

        if (m_activeTemplate == null)
        {
            Debug.LogError("template not found!");
        }
    }

    public SetupMetadataState(ModelRef existingModelRef, EditorWindow window, StateMachine owner) : base(window, owner)
    {
        m_modelRef = existingModelRef;

        string path = AssetDatabase.GetAssetPath(m_modelRef);
        if(path == null)
            Debug.LogError("failed to find metadata file");
        m_importConfig = new ImportConfig();
        m_importConfig.AssetPath = Path.GetDirectoryName(path);
        m_importConfig.AssetName = Path.GetFileNameWithoutExtension(path);
        Debug.Log("asset path: " + m_importConfig.AssetPath);
        Debug.Log("asset name: " + m_importConfig.AssetName);
    }

    public override void Update()
    {
        if (m_modelRef == null)
            return;

        m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);

        GUILayout.Space(20);

        GUILayout.Label("Metadata Configuration", EditorStylesHelper.TitleStyle);

        GUILayout.Space(20);

        //at top level
        if (m_parentModelData == null)
        {
            ShowModelData(m_modelRef.Root);
        }
        else
        {
            foreach (var mChild in m_parentModelData.SubModels)
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

        ////description
        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Space(20);
        //EditorGUILayout.LabelField("Description: ", EditorStylesHelper.LabelStyle);
        //modelData.Description = EditorGUILayout.TextField(modelData.Description, GUILayout.Width(300));
        //GUILayout.EndHorizontal();

        ////URL
        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Space(20);
        //EditorGUILayout.LabelField("URL: ", EditorStylesHelper.LabelStyle);
        //modelData.Url = EditorGUILayout.TextField(modelData.Url, GUILayout.Width(300));
        //GUILayout.EndHorizontal();

        ////image
        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Space(20);
        //EditorGUILayout.LabelField("Image: ", EditorStylesHelper.LabelStyle);
        //modelData.Image = (Image)EditorGUILayout.ObjectField(modelData.Image, typeof(Image), false, GUILayout.Width(300));
        //GUILayout.EndHorizontal();

        ////video clip
        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Space(20);
        //EditorGUILayout.LabelField("Video: ", EditorStylesHelper.LabelStyle);
        //modelData.Video = (VideoClip)EditorGUILayout.ObjectField(modelData.Video, typeof(VideoClip), false, GUILayout.Width(300));
        //GUILayout.EndHorizontal();


        //sub model button
        if (modelData.Transform.childCount == 0)
            return;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Go to its sub-models? ", EditorStylesHelper.LabelStyle);
        if (GUILayout.Button("Go", EditorStylesHelper.RegularButtonStyle, GUILayout.Width(40)))
        {
            CreateModelDataForEachSubModel(modelData.Transform, modelData);
            m_parentModelData = modelData;
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
        string absDstPath = m_importConfig.AssetPath;
        string relDstPath = absDstPath.Replace(Application.dataPath, "Assets");
        string relModelPath = EditorUtilities.CombinePaths(relDstPath, m_importConfig.AssetName) + ".FBX";
        AssetDatabase.ImportAsset(
            relModelPath,
            ImportAssetOptions.ForceUpdate);

        if (m_importConfig.AlbedoMapPath != null && File.Exists(m_importConfig.AlbedoMapPath))
        {
            string relAlbedoPath = EditorUtilities.CombinePaths(relDstPath, k_albedoMapName) + "." + m_importConfig.AlbedoMapPath.Split('.').Last();
            Debug.Log(relAlbedoPath);
            AssetDatabase.ImportAsset(
                relAlbedoPath,
                ImportAssetOptions.ForceUpdate);
        }

        if (m_importConfig.NormalMapPath != null && File.Exists(m_importConfig.NormalMapPath))
        {
            string relNormalPath = EditorUtilities.CombinePaths(relDstPath, k_normalMapName) + "." + m_importConfig.NormalMapPath.Split('.').Last();
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
        var modelInstance = GameObject.Instantiate(model);

        //create model ref
        m_modelRef = ScriptableObject.CreateInstance<ModelRef>();
        m_modelRef.Root = new ModelData();
        m_modelRef.Root.Transform = model.transform;
        m_modelRef.Model = modelInstance;
    }


    private void CopyAllResources()
    {
        File.Copy(
            m_importConfig.ResourcePath,
            EditorUtilities.CombinePaths(m_importConfig.AssetPath, m_importConfig.AssetName) + ".FBX",
            true);

        if (m_importConfig.AlbedoMapPath != null && File.Exists(m_importConfig.AlbedoMapPath))
        {
            string albedoPath = EditorUtilities.CombinePaths(m_importConfig.AssetPath, k_albedoMapName) + "." + m_importConfig.AlbedoMapPath.Split('.').Last();
            File.Copy(
                m_importConfig.AlbedoMapPath,
                albedoPath,
                true);
        }

        if (m_importConfig.NormalMapPath != null && File.Exists(m_importConfig.NormalMapPath))
        {
            string normalPath = EditorUtilities.CombinePaths(m_importConfig.AssetPath, k_normalMapName) + "." + m_importConfig.NormalMapPath.Split('.').Last();
            File.Copy(
                m_importConfig.NormalMapPath,
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
            mChild.Index = i;
            mChild.Transform = tChild;
            mParent.SubModels.Add(mChild);
            mChild.Parent = mParent; 
        }
    }

    private void OnBackButtonClicked()
    {
        if (m_parentModelData == null) 
        {
            SelectModelState state = new SelectModelState(EditorWindow, Owner);
            ChangeState(state);
        }
        else
        {
            m_parentModelData = m_parentModelData.Parent;
            EditorWindow.Repaint();
        }
    }


    private void OnSaveButtonClicked()
    {
        CalculateSizeAndCenter(m_modelRef.Root);

        string absDstPath = EditorUtilities.CombinePaths(m_importConfig.AssetPath, m_importConfig.AssetName) + ".asset";
        string relDstPath = absDstPath.Replace(Application.dataPath, "Assets");
        m_modelRef.Root.MetadataList.Add(new ImageFieldData());

        // If asset already exists at the path, delete it
        if (AssetDatabase.LoadAssetAtPath(relDstPath, typeof(ModelRef)) != null)
        {
            EditorUtility.SetDirty(m_modelRef);
            AssetDatabase.SaveAssets();
        }
        else
        {
            AssetDatabase.CreateAsset(m_modelRef, relDstPath);
            AssetDatabase.SaveAssets();
        }

        // save prefab
        // Add your component
        MetadataInspector inspector = m_modelRef.Model.AddComponent<MetadataInspector>();
        inspector.ModelRef = m_modelRef;

        // Save the modified prefab
        PrefabUtility.SaveAsPrefabAsset(m_modelRef.Model, "Assets/model.prefab");

        // destory model instance
        GameObject.DestroyImmediate(m_modelRef.Model);
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
