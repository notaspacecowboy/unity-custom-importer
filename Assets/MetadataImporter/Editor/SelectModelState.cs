using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SelectModelState : IImportWindowState
{
    private GameObject m_selectedSceneGameObject;
    private ModelRef m_existingProfile;
    private ModelRef m_existingProfileParOnly;

    public SelectModelState(EditorWindow window, StateMachine owner) : base(window, owner, 450, 250, 300)
    {
    }

    public override void Update()
    {
        base.Update();

        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUILayout.Label("Custom Model Importer", EditorStylesHelper.TitleStyle);
        GUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Import a new FBX model or choose from existing profile: ", EditorStylesHelper.LabelStyle, GUILayout.Width(400));
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        //file picker
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Open File Browser: ", EditorStylesHelper.LabelStyle, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace));
        if (GUILayout.Button("...", GUILayout.Width(110)))
            OnFilePickerButtonClicked();
        GUILayout.EndHorizontal();

        //pick a game object in the scene
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Select a game object in the scene: ", EditorStylesHelper.LabelStyle, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace));
        m_selectedSceneGameObject = (GameObject)EditorGUILayout.ObjectField("", m_selectedSceneGameObject, typeof(GameObject), true, GUILayout.Width(110));
        if (m_selectedSceneGameObject != null)
        {
            OnSceneGameObjectSelected();
        }
        GUILayout.EndHorizontal();

        //pick an existing profile
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Continue with an existing profile: ", EditorStylesHelper.LabelStyle, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace));
        m_existingProfile = (ModelRef)EditorGUILayout.ObjectField("", m_existingProfile, typeof(ModelRef), false, GUILayout.Width(110));
        if (m_existingProfile != null)
        {
            OnImportExistingProfile(m_existingProfile, false);
        }
        GUILayout.EndHorizontal();

        //pick an existing profile and only allows user to modify paradata
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Continue with an existing profile (paradata only): ", EditorStylesHelper.LabelStyle, GUILayout.Width(m_minHorizontalSpace + m_extraHorSpace));
        m_existingProfileParOnly = (ModelRef)EditorGUILayout.ObjectField("", m_existingProfileParOnly, typeof(ModelRef), false, GUILayout.Width(110));
        if (m_existingProfileParOnly != null)
        {
            OnImportExistingProfile(m_existingProfileParOnly, true);
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(30);
        EditorWindow.minSize = new Vector2(450, 250);
    }

    private void OnFilePickerButtonClicked()
    {
        string path = EditorUtility.OpenFilePanel("Select a model file", "", "fbx");

        if (string.IsNullOrEmpty(path))
            return;

        // Verify if the dropped object is a model (currently only supporting fbx)
        if (!path.Split('.')[path.Split('.').Length - 1].ToLower().Equals("fbx"))
            return;

        var state = new SetConfigState(path, EditorWindow, Owner);
        ChangeState(state);
    }


    private void OnSceneGameObjectSelected()
    {
        var state = new SetConfigState(m_selectedSceneGameObject, EditorWindow, Owner);
        ChangeState(state);
    }


    private void OnImportExistingProfile(ModelRef model, bool paradataOnly)
    {
        var state = new SetupMetadataState(model, paradataOnly, EditorWindow, Owner);
        ChangeState(state);
    }
}
