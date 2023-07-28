using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


public class SetConfigState : IImportWindowState
{
    private enum ImportType
    {
        ImportModel,
        AttachToSceneGameObject
    }


    #region constants

    private const string k_resourceNotFoundErr = "Resource file does not exist!";
    private const string k_assetPathNotExistErr = "Resource file does not exist!";
    private const string k_emptyAssetNameErr = "Resource file does not exist!";
    private const string k_invalidTemplateErr = "Metadata template is invalid!";
    private const float  k_labelWidth = 300;

    #endregion

    private EditorImportConfig m_ImportConfig = new EditorImportConfig();
    private string[] m_options = {};
    private int m_selectedIndex = 0;
    private ImportType m_importType = ImportType.ImportModel;
    private GameObject m_sceneGameObject;
    
    public SetConfigState(string resourcePath, EditorWindow window, StateMachine owner) : base(window, owner, 700, 300, 300)
    {
        m_ImportConfig.Reset();
        m_ImportConfig.ResourcePath = resourcePath;

        m_importType = ImportType.ImportModel;
    }

    public SetConfigState(GameObject sceneObj, EditorWindow window, StateMachine owner) : base(window, owner, 700, 300,
        300)
    {
        m_ImportConfig.Reset();
        m_sceneGameObject = sceneObj;

        m_importType = ImportType.AttachToSceneGameObject;
    }


    public override void Update()
    {
        base.Update();

        RegenerateTemplateOptions();

        GUILayout.Space(20);

        GUILayout.Label("Model & Material Configuration", EditorStylesHelper.TitleStyle);

        GUILayout.Space(20);

        //resource path
        if (m_importType == ImportType.ImportModel)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Model Path: ", EditorStylesHelper.LabelStyle, GUILayout.Width(k_labelWidth));
            EditorGUILayout.LabelField(m_ImportConfig.ResourcePath, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace));
            GUILayout.Space(20);
            if (GUILayout.Button("...", GUILayout.Width(25)))
            {
                m_ImportConfig.ResourcePath = EditorUtility.OpenFilePanel("Choose Resource File", ".", "fbx");
                EditorWindow.Repaint();
            }
            GUILayout.EndHorizontal();
        }

        //asset path
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Asset Path: ", EditorStylesHelper.LabelStyle, GUILayout.Width(k_labelWidth));
        EditorGUILayout.LabelField(m_ImportConfig.AssetPath, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace));
        GUILayout.Space(20);
        if (GUILayout.Button("...", GUILayout.Width(25)))
        {
            m_ImportConfig.AssetPath = EditorUtility.OpenFolderPanel("Choose Model Directory", ".", "");
            EditorWindow.Repaint();
        }
        GUILayout.EndHorizontal();

        //asset name
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Asset Name: ", EditorStylesHelper.LabelStyle, GUILayout.Width(k_labelWidth));
        m_ImportConfig.AssetName = EditorGUILayout.TextField(m_ImportConfig.AssetName, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace + 48));
        GUILayout.EndHorizontal();

        if (m_importType == ImportType.ImportModel)
        {
            //albedo map path
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Albedo Map Path: ", EditorStylesHelper.LabelStyle, GUILayout.Width(k_labelWidth));
            EditorGUILayout.LabelField(m_ImportConfig.AlbedoMapPath, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace));
            GUILayout.Space(20);
            if (GUILayout.Button("...", GUILayout.Width(25)))
            {
                m_ImportConfig.AlbedoMapPath = EditorUtility.OpenFilePanel("Choose Resource File", ".", "jpg,jpeg,png");
                EditorWindow.Repaint();
            }
            GUILayout.EndHorizontal();

            //normal map path
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Normal Map Path: ", EditorStylesHelper.LabelStyle, GUILayout.Width(k_labelWidth));
            EditorGUILayout.LabelField(m_ImportConfig.NormalMapPath, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace));
            GUILayout.Space(20);
            if (GUILayout.Button("...", GUILayout.Width(25)))
            {
                m_ImportConfig.NormalMapPath = EditorUtility.OpenFilePanel("Choose Resource File", ".", "jpg,jpeg,png");
                EditorWindow.Repaint();
            }
            GUILayout.EndHorizontal();
        }


        //choose a template
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Choose a new metadata template: ", EditorStylesHelper.LabelStyle, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace  + 145));
        if (GUILayout.Button("Import", GUILayout.Width(100)))
        {
            string newTemplatePath = EditorUtility.OpenFilePanel("Import new template", ".", "csv");
            if (!string.IsNullOrEmpty(newTemplatePath))
                ImportNewMetadataTemplate(newTemplatePath);
        }
        m_selectedIndex = EditorGUILayout.Popup(m_selectedIndex, m_options, GUILayout.Width(100));
        GUILayout.EndHorizontal();


        GUILayout.Space(30);


        //back button
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        if (GUILayout.Button("Back", EditorStylesHelper.RegularButtonStyle, GUILayout.Height(20), GUILayout.Width(650 + m_extraHorSpace)))
        {
            ChangeState(new SelectModelState(EditorWindow, Owner));
        }
        EditorGUILayout.EndHorizontal();

        //validate button
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        if (GUILayout.Button("Validate", EditorStylesHelper.WarnButtonStyle, GUILayout.Height(20), GUILayout.Width(650 + m_extraHorSpace)))
            Validate();
        EditorGUILayout.EndHorizontal();

        ShowErrorMessage();

        //create button
        if (m_ImportConfig.Validated)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            if (GUILayout.Button("Create", EditorStylesHelper.RegularButtonStyle, GUILayout.Height(20), GUILayout.Width(650 + m_extraHorSpace)))
            {
                m_ImportConfig.TemplateName = m_options[m_selectedIndex];
                SetupMetadataState state = null;
                if (m_importType == ImportType.ImportModel)
                    state = new SetupMetadataState(m_ImportConfig, EditorWindow, Owner);
                else
                    state = new SetupMetadataState(m_sceneGameObject, m_ImportConfig, EditorWindow, Owner);
                ChangeState(state);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void ImportNewMetadataTemplate(string csvPath)
    {
        TemplateImporter.Instance.Import(csvPath);
    }
    private void RegenerateTemplateOptions()
    {
        var templates = TemplateImporter.Instance.Template.Templates;
        if (m_options.Length == templates.Count)
            return;

        m_options = new string[templates.Count];
        for (int i = 0; i < templates.Count; i++)
            m_options[i] = templates[i].Name;
    }

    private void Validate()
    {
        if (m_importType == ImportType.ImportModel && !System.IO.File.Exists(m_ImportConfig.ResourcePath))
            m_ImportConfig.ErrorMessage = k_resourceNotFoundErr;
        else if (!System.IO.Directory.Exists(m_ImportConfig.AssetPath))
            m_ImportConfig.ErrorMessage = k_assetPathNotExistErr;
        else if (string.IsNullOrEmpty(m_ImportConfig.AssetName))
            m_ImportConfig.ErrorMessage = k_emptyAssetNameErr;
        else if (m_options.Length == 0 || m_selectedIndex >= m_options.Length)
        {
            m_ImportConfig.ErrorMessage = k_invalidTemplateErr;
        }
        else
        {
            m_ImportConfig.Validated = true;
            m_ImportConfig.ErrorMessage = "";
        }
    }

    private void ShowErrorMessage()
    {
        //error message
        if (!m_ImportConfig.Validated && m_ImportConfig.ErrorMessage != "")
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(m_ImportConfig.ErrorMessage, EditorStylesHelper.ErrorLabelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        //success message
        if (m_ImportConfig.Validated)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Setup has been validated!", EditorStylesHelper.SuccessLabelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}
