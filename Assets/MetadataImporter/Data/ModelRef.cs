using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TMPro;using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif



public interface IJsonObject
{
    public JObject ToJson();
}

public interface IFieldVisualizer
{
}

public interface IStringFieldVisualizer: IFieldVisualizer
{
    TextMeshProUGUI StringField { get; }
}

public interface IImageFieldVisualizer : IFieldVisualizer
{
    Image ImageField { get; }
}

public interface IVideoFieldVisualizer : IFieldVisualizer
{
    VideoPlayer VideoField { get; }
}

[Serializable]
public class FieldData: IJsonObject
{
    [SerializeField]
    public string FieldName;

    [SerializeField]
    public bool IsParaData;

#if UNITY_EDITOR
    public virtual void OnGUI(GUIStyle style, params GUILayoutOption[] options) {}
#endif

    public virtual void ShowField(IFieldVisualizer visualizer) {}

    public virtual JObject ToJson()
    {
        return new JObject();
    }
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
        EditorGUILayout.LabelField($"{FieldName}{(IsParaData ? "*" : "")}: ", style);
        FieldValue = EditorGUILayout.TextField(FieldValue, options);
        GUILayout.EndHorizontal();
    }
#endif

    public override JObject ToJson()
    {
        return new JObject
        {
            ["FieldName"] = FieldName,
            ["FieldValue"] = FieldValue ?? "",
            ["IsParaData"] = IsParaData
        };
    }


    public override void ShowField(IFieldVisualizer visualizer)
    {
        if (visualizer is IStringFieldVisualizer stringFieldVisualizer && FieldValue != null)
        {
            stringFieldVisualizer.StringField.text = FieldValue;
            stringFieldVisualizer.StringField.gameObject.SetActive(true);
        }
    }
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
        EditorGUILayout.LabelField($"{FieldName}{(IsParaData ? "*" : "")}: ", style);
        FieldValue = (Texture2D)EditorGUILayout.ObjectField(FieldValue, typeof(Texture2D), false, options);
        GUILayout.EndHorizontal();
    }
#endif

    public override void ShowField(IFieldVisualizer visualizer)
    {
        if (visualizer is IImageFieldVisualizer imageFieldVisualizer && FieldValue != null)
        {
            imageFieldVisualizer.ImageField.sprite = Sprite.Create(FieldValue, new Rect(0, 0, FieldValue.width, FieldValue.height), Vector2.zero);
            imageFieldVisualizer.ImageField.SetNativeSize();
            imageFieldVisualizer.ImageField.gameObject.SetActive(true);
        }
    }

    public override JObject ToJson()
    {
        return new JObject
        {
            ["FieldName"] = FieldName,
            ["FieldValue"] = (FieldValue == null ? "" : FieldValue.name),
            ["IsParaData"] = IsParaData
        };
    }
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
        EditorGUILayout.LabelField($"{FieldName}{(IsParaData ? "*" : "")}: ", style);
        FieldValue = (VideoClip)EditorGUILayout.ObjectField(FieldValue, typeof(VideoClip), false, options);
        GUILayout.EndHorizontal();
    }
#endif

    public override void ShowField(IFieldVisualizer visualizer)
    {
        if (visualizer is IVideoFieldVisualizer videoFieldVisualizer && FieldValue != null)
        {
            videoFieldVisualizer.VideoField.clip = FieldValue;
            videoFieldVisualizer.VideoField.gameObject.SetActive(true);
        }
    }

    public override JObject ToJson()
    {
        return new JObject
        {
            ["FieldName"] = FieldName,
            ["FieldValue"] = (FieldValue == null ? "" : FieldValue.name),
            ["IsParaData"] = IsParaData
        };
    }
}


[Serializable]
public class ModelData: IJsonObject
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

    [SerializeReference] 
    private Material m_highlightMaterial;

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

    public Material HighlightMaterial
    {
        get => m_highlightMaterial;
        set => m_highlightMaterial = value;
    }

    public Transform Transform { get; set; }

    public BoxCollider Collider { get; set; }


    public ModelData()
    {
        m_name = "";
        m_subModels = new List<ModelData>();
        m_metadataList = new List<FieldData>();
    }

    public JObject ToJson()
    {
        JObject jObject = new JObject
        {
            ["Name"] = Name,
            ["Index"] = Index,
            ["SubModels"] = new JArray(SubModels.Select(subModel => subModel.ToJson())),
            ["MetadataList"] = new JArray(MetadataList.Select(metadata => metadata.ToJson())),
            ["HighlightMaterial"] = (HighlightMaterial == null ? "" : HighlightMaterial.name)
        };
        return jObject;
    }
}


[Serializable]
public class ModelRef : ScriptableObject, IJsonObject
{
    [SerializeField]
    private GameObject m_go;

    [SerializeField]
    private ModelData m_root;

    [SerializeField]
    private string m_templateName;

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

    public string TemplateName
    {
        get => m_templateName;
        set => m_templateName = value;
    }

    public JObject ToJson()
    {
        JObject jObject = new JObject
        {
            ["Root"] = Root.ToJson()
        };
        return jObject;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(ModelRef))]
public class ModelRefEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default inspector

        ModelRef modelRef = (ModelRef)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Export Metadata"))
        {
            string exportPath = EditorUtility.OpenFolderPanel("Choose Export Path", ".", "");
            ExportToPath(modelRef, exportPath);
        }
    }

    public void ExportToPath(ModelRef modelRef, string exportPath)
    {
        string json = modelRef.ToJson().ToString();
        File.WriteAllText(exportPath + "/metadata.json", json);
    }
}
#endif
