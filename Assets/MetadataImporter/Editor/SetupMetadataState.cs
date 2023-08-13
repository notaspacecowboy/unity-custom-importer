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
    private enum ImportType
    {
        ImportModel,
        AttachToSceneGameObject,
        ContinueWithExistingMetaData,
        ContinueWithExistingParadata
    }


    #region fields

    private static readonly string k_albedoMapName = "albedo";
    private static readonly string k_normalMapName = "normal";

    private EditorImportConfig m_importConfig;

    private ModelRef m_modelRef;

    private Vector2 m_scrollPosition;

    private ModelData m_parentModelData;

    private MetadataTemplate m_activeTemplate;

    private ImportType m_importType = ImportType.ImportModel;

    #endregion

    #region lifecycle methods

    public SetupMetadataState(EditorImportConfig config, EditorWindow window, StateMachine owner) : base(window, owner, 600, 600, 300)
    {
        m_importType = ImportType.ImportModel;

        m_importConfig = config;

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
            return;
        }

        LoadModelAndExtractMaterial().Forget();
    }

    public SetupMetadataState(GameObject selectedSceneGameObject, EditorImportConfig config, EditorWindow window, StateMachine owner) : base(window, owner, 600, 600, 300)
    {
        m_importType = ImportType.AttachToSceneGameObject;

        m_importConfig = config;

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
            return;
        }

        //create model ref
        m_modelRef = ScriptableObject.CreateInstance<ModelRef>();
        m_modelRef.Root = new ModelData();
        m_modelRef.Root.Transform = selectedSceneGameObject.transform;
        m_modelRef.GameObject = selectedSceneGameObject;
        m_modelRef.TemplateName = m_importConfig.TemplateName;
        m_modelRef.UncertaintyShader = m_importConfig.Shader;

        AttachFieldToModel(m_modelRef.Root);
    }

    public SetupMetadataState(ModelRef existingModelRef, bool paradataOnly, EditorWindow window, StateMachine owner) : base(window, owner, 600, 600, 300)
    {
        m_importType = paradataOnly ? ImportType.ContinueWithExistingParadata : ImportType.ContinueWithExistingMetaData;

        string path = AssetDatabase.GetAssetPath(existingModelRef);
        if (path == null)
            Debug.LogError("failed to find metadata file");
        m_importConfig = new EditorImportConfig();
        m_importConfig.AssetPath = Path.GetDirectoryName(path);
        m_importConfig.AssetName = Path.GetFileNameWithoutExtension(path);
        m_importConfig.TemplateName = existingModelRef.TemplateName;
        m_importConfig.ParadataOnly = paradataOnly;
        m_importConfig.Shader = existingModelRef.UncertaintyShader;

        foreach (var template in TemplateImporter.Instance.Template.Templates)
        {
            if (template.Name == m_importConfig.TemplateName)
            {
                m_activeTemplate = template;
                break;
            }
        }

        LoadExistingModel(existingModelRef).Forget();
    }

    public override void OnLeave()
    {
        Debug.Log("on leave");

        base.OnLeave();


        if (m_modelRef != null && m_modelRef.GameObject != null)
            GameObject.DestroyImmediate(m_modelRef.GameObject);
    }

    #endregion

    #region GUI

    public override void Update()
    {
        if (m_modelRef == null)
            return;

        base.Update();

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
        EditorStylesHelper.HorizontalCenteredButton(OnBackButtonClicked, "Back", EditorStylesHelper.WarnButtonStyle, GUILayout.Height(30), GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace + 100));
        EditorStylesHelper.HorizontalCenteredButton(OnSaveButtonClicked, "Save", EditorStylesHelper.RegularButtonStyle, GUILayout.Height(30), GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace + 100));
        GUILayout.Space(40);
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
        if (!m_importConfig.ParadataOnly)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Name: ", EditorStylesHelper.LabelStyle);
            modelData.Name = EditorGUILayout.TextField(modelData.Name, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace));
            GUILayout.EndHorizontal();
        }

        //uncertainty level
        if(!m_importConfig.ParadataOnly)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Uncertainty Level: ", EditorStylesHelper.LabelStyle);
            modelData.UncertaintyLevel = EditorGUILayout.Slider(modelData.UncertaintyLevel, 0, 100, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace));
            GUILayout.EndHorizontal();
        }

        //show metadata fields
        foreach (var field in modelData.MetadataList)
        {
            if (!field.IsParaData && m_importConfig.ParadataOnly)
                continue;
            field.OnGUI(EditorStylesHelper.LabelStyle, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace));
        }



        //sub model button
        if (modelData.Transform.childCount == 0)
            return;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Go to its sub-models? ", EditorStylesHelper.LabelStyle);
        if (GUILayout.Button("Go", EditorStylesHelper.RegularButtonStyle, GUILayout.Width(40)))
        {
            OnGoButtonClicked(modelData);
            EditorWindow.Repaint();
        }
        GUILayout.EndHorizontal();
    }

    #endregion

    #region import model and create material

    private async UniTask LoadExistingModel(ModelRef existingModelRef)
    {
        string absDstPath = m_importConfig.AssetPath;
        string relDstPath = absDstPath.Replace(Application.dataPath, "Assets");
        string relModelPath = EditorUtilities.CombinePaths(relDstPath, "prefab_" + m_importConfig.AssetName) + ".prefab";

        var model = AssetDatabase.LoadAssetAtPath<GameObject>(relModelPath);
        var modelInstance = GameObject.Instantiate(model);
        existingModelRef.GameObject = modelInstance;

        await UniTask.DelayFrame(1);
        FixModelReference(modelInstance.transform, existingModelRef.Root);
        await UniTask.DelayFrame(1);

        m_modelRef = existingModelRef;
    }

    private void FixModelReference(Transform transform, ModelData model)
    {
        model.Transform = transform;
        foreach (var subModel in model.SubModels)
            FixModelReference(transform.GetChild(subModel.Index), subModel);
    }

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
        m_modelRef.GameObject = modelInstance;
        m_modelRef.TemplateName = m_importConfig.TemplateName;
        m_modelRef.UncertaintyShader = m_importConfig.Shader;

        AttachFieldToModel(m_modelRef.Root);
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

    private void CreateModelDataForEachSubModel(Transform tParent, ModelData mParent)
    {
        if (mParent.SubModels.Count != 0)
            return;

        for (int i = 0; i < tParent.childCount; i++)
        {
            Transform tChild = tParent.GetChild(i);
            if(tChild.GetComponentsInChildren<Renderer>().Length == 0)
                continue;

            var mChild = new ModelData();
            mChild.Index = i;
            mChild.Transform = tChild;
            mParent.SubModels.Add(mChild);
            mChild.Parent = mParent;
            AttachFieldToModel(mChild);
        }
    }

    private void AttachFieldToModel(ModelData model)
    {
        foreach (var field in m_activeTemplate.Fields)
        {
            switch (field.Type)
            {
                case FieldType.String:
                    model.MetadataList.Add(new StringFieldData() { FieldName = field.Name, IsParaData = field.IsParaData});
                    break;

                case FieldType.Image:
                    model.MetadataList.Add(new ImageFieldData() { FieldName = field.Name, IsParaData = field.IsParaData });
                    break;

                case FieldType.VideoClip:
                    model.MetadataList.Add(new VideoFieldData() { FieldName = field.Name, IsParaData = field.IsParaData });
                    break;

                default:
                    break;
            }
        }
    }

    private void AddSubmodelHighlighter(Transform rootTransform, ModelData rootModel)
    {
        rootTransform.AddComponent<SubModelHighlighter>();
        foreach (var model in rootModel.SubModels)
            AddSubmodelHighlighter(rootTransform.GetChild(model.Index), model);
    }

    private void AddColliders(Transform rootTransform, ModelData rootModel)
    {
        foreach (var model in rootModel.SubModels)
            AddColliders(rootTransform.GetChild(model.Index), model);

        // Store the original transform properties
        Vector3 originalPosition = rootTransform.position;
        Quaternion originalRotation = rootTransform.rotation;
        Vector3 originalScale = rootTransform.localScale;

        // Temporarily negate position, rotation, and scale
        rootTransform.position = Vector3.zero;
        rootTransform.rotation = Quaternion.identity;
        rootTransform.localScale = Vector3.one;

        List<Bounds> objectsBounds = rootTransform.GetComponentsInChildren<Renderer>().Select(r => r.bounds).ToList();

        float minX = objectsBounds.Min(bound => bound.min.x);
        float minY = objectsBounds.Min(bound => bound.min.y);
        float minZ = objectsBounds.Min(bound => bound.min.z);

        float maxX = objectsBounds.Max(bound => bound.max.x);
        float maxY = objectsBounds.Max(bound => bound.max.y);
        float maxZ = objectsBounds.Max(bound => bound.max.z);

        BoxCollider collider = rootTransform.AddComponent<BoxCollider>();
        collider.center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);
        collider.size = new Vector3(maxX - minX, maxY - minY, maxZ - minZ);

        if (m_importType == ImportType.AttachToSceneGameObject)
        {
            collider.center -= rootTransform.position;
            
        }

        collider.enabled = false;

        // Restore the original transform properties
        rootTransform.position = originalPosition;
        rootTransform.rotation = originalRotation;
        rootTransform.localScale = originalScale;
    }


    #endregion

    #region button callbacks

    private void OnGoButtonClicked(ModelData modelData)
    {
        CreateModelDataForEachSubModel(modelData.Transform, modelData);
        m_parentModelData = modelData;
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
        string absDstPath = EditorUtilities.CombinePaths(m_importConfig.AssetPath, m_importConfig.AssetName) + ".asset";
        string relDstPath = absDstPath.Replace(Application.dataPath, "Assets");

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
        if (m_importType != ImportType.ContinueWithExistingMetaData &&
            m_importType != ImportType.ContinueWithExistingParadata)
        {
            MetadataComponent inspector = m_modelRef.GameObject.AddComponent<MetadataComponent>();
            AddSubmodelHighlighter(m_modelRef.GameObject.transform, m_modelRef.Root);
            AddColliders(m_modelRef.GameObject.transform, m_modelRef.Root);
            inspector.ModelRef = m_modelRef;
        }

        // Save the modified prefab
        string prefabPath = EditorUtilities.CombinePaths(m_importConfig.AssetPath, "prefab_" + m_importConfig.AssetName) + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(m_modelRef.GameObject, prefabPath);

        // destory model instance
        //GameObject.DestroyImmediate(m_modelRef.GameObject);
    }

    #endregion
}
