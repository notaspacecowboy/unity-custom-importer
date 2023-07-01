using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class FieldData
{
    public string FieldName;

#if UNITY_EDITOR
    public virtual void OnGUI(GUIStyle style, params GUILayoutOption[] options) {}
#endif
}

[System.Serializable]
public class StringFieldData : FieldData
{
    [SerializeField]
    public string FieldValue;

#if UNITY_EDITOR
    public override void OnGUI(GUIStyle style, params GUILayoutOption[] options)
    {
        base.OnGUI(style, options);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField($"{FieldName}: ", style);
        FieldValue = EditorGUILayout.TextField(FieldValue, options);
        GUILayout.EndHorizontal();
    }

#endif
}

[System.Serializable]
public class ImageFieldData : FieldData
{
    [SerializeField]
    public Texture2D FieldValue;

#if UNITY_EDITOR
    public override void OnGUI(GUIStyle style, params GUILayoutOption[] options)
    {
        base.OnGUI(style, options);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField($"{FieldName}: ", style);
        FieldValue = (Texture2D)EditorGUILayout.ObjectField(FieldValue, typeof(Texture2D), false, options);
        GUILayout.EndHorizontal();
    }
#endif
}

[System.Serializable]
public class VideoFieldData : FieldData
{
    [SerializeField]
    public VideoClip FieldValue;

#if UNITY_EDITOR
    public override void OnGUI(GUIStyle style, params GUILayoutOption[] options)
    {
        base.OnGUI(style, options);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        EditorGUILayout.LabelField($"{FieldName}: ", style);
        FieldValue = (VideoClip)EditorGUILayout.ObjectField(FieldValue, typeof(VideoClip), false, options);
        GUILayout.EndHorizontal();
    }
#endif
}


[Serializable]
public class ModelData
{
    [SerializeField]
    private string m_name;

    [SerializeField]
    private int m_index;

    [SerializeField]
    private ModelData m_parent;

    [SerializeField]
    private List<ModelData> m_subModels;

    //actual metadata
    [SerializeReference]
    private List<FieldData> m_metadataList;

    public int Index
    {
        get => m_index;
        set => m_index = value;
    }

    public string Name
    {
        get
        {
            if (m_name == "")
                return Transform.name;
            return m_name;
        }
        set => m_name = value;
    }

    public ModelData Parent
    {
        get => m_parent;
        set => m_parent = value;
    }
    
    public List<ModelData> SubModels
    {
        get => m_subModels;
        set => m_subModels = value;
    }
    public List<FieldData> MetadataList
    {
        get => m_metadataList;
        set => m_metadataList = value;
    }

    public Transform Transform { get; set; }

    public BoxCollider Collider { get; set; }

    public ModelData()
    {
        m_name = "";
        m_subModels = new List<ModelData>();
        m_metadataList = new List<FieldData>();
    }
}


[Serializable]
public class ModelRef : ScriptableObject
{
    [SerializeField]
    private GameObject m_go;

    [SerializeField]
    private ModelData m_root;

    public GameObject GameObject
    {
        get => m_go;
        set => m_go = value;
    }
    public ModelData Root
    {
        get => m_root;
        set => m_root = value;
    }
}
