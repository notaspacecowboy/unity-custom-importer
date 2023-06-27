using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Object = UnityEngine.Object;


[Serializable]
public class FieldData
{
    public string FieldName;
}

[System.Serializable]
public class StringFieldData : FieldData
{
    [SerializeField]
    public string FieldValue;
}

[System.Serializable]
public class ImageFieldData : FieldData
{
    [SerializeField]
    public Texture2D FieldValue;
}

[System.Serializable]
public class VideoFieldData : FieldData
{
    [SerializeField]
    public VideoClip FieldValue;
}


[Serializable]
public class ModelData
{
    [SerializeField]
    private string m_name;

    [SerializeField]
    private int m_index;

    [SerializeField]
    private Transform m_transform;

    [SerializeField]
    private ModelData m_parent;

    [SerializeField]
    private Vector3 m_size;

    [SerializeField]
    private Vector3 m_center;

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

    public Transform Transform
    {
        get => m_transform;
        set => m_transform = value;
    }

    public ModelData Parent
    {
        get => m_parent;
        set => m_parent = value;
    }

    public Vector3 Size
    {
        get => m_size;
        set => m_size = value;
    }

    public Vector3 Center
    {
        get => m_center;
        set => m_center = value;
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
    private GameObject _mModel;

    [SerializeField]
    private ModelData _mRoot;

    public GameObject Model
    {
        get => _mModel;
        set => _mModel = value;
    }
    public ModelData Root
    {
        get => _mRoot;
        set => _mRoot = value;
    }
}
