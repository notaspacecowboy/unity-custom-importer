using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SelectModelState : IImportWindowState
{
    private ModelRef _existingProfile = null;

    public SelectModelState(EditorWindow window, StateMachine owner) : base(window, owner)
    {
    }

    public override void Update()
    {
        GUILayout.Space(20);

        GUILayout.Label("Custom Model Importer", EditorStylesHelper.TitleStyle);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        var style = EditorStylesHelper.LabelStyle;
        style.normal.textColor = EditorStylesHelper.LightBlue;
        EditorGUILayout.LabelField("Import a new FBX model or choose from existing profile: ", style, GUILayout.Width(400));
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        //file picker
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Open File Browser: ", EditorStylesHelper.LabelStyle, GUILayout.Width(300));
        if (GUILayout.Button("...", GUILayout.Width(110)))
            OnFilePickerButtonClicked();
        GUILayout.EndHorizontal();

        //pick an existing profile
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField("Continue with an existing profile: ", EditorStylesHelper.LabelStyle, GUILayout.Width(300));
        _existingProfile = (ModelRef)EditorGUILayout.ObjectField("", _existingProfile, typeof(ModelRef), false, GUILayout.Width(110));
        if (_existingProfile != null)
        {
            OnImportExistingProfile();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(30);
        EditorWindow.minSize = EditorWindow.maxSize = new Vector2(450, 250);
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

    private void OnImportExistingProfile()
    {
        var state = new SetupMetadataState(_existingProfile, EditorWindow, Owner);
        ChangeState(state);
    }
}
