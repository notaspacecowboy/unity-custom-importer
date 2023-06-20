using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


[Serializable]
public class ModelData
{
    [SerializeField]
    private Transform _mTransform;

    [SerializeField]
    private string _mName;

    [SerializeField]
    private string _mDescription;

    [SerializeField]
    private string _mUrl;

    [SerializeField]
    private Image _mImage;

    [SerializeField]
    private VideoClip _mVideo;

    [SerializeField]
    private ModelData _mParent;

    [SerializeField]
    private Vector3 _mSize;

    [SerializeField]
    private Vector3 _mCenter;

    [SerializeField]
    private List<ModelData> _mSubModels;

    public Transform Transform
    {
        get => _mTransform;
        set => _mTransform = value;
    }

    public string Name
    {
        get
        {
            if (_mName == "")
                return Transform.name;
            return _mName;
        }
        set => _mName = value;
    }

    public string Description
    {
        get => _mDescription;
        set => _mDescription = value;
    }

    public string Url
    {
        get => _mUrl;
        set => _mUrl = value;
    }

    public Image Image
    {
        get => _mImage;
        set => _mImage = value;
    }

    public VideoClip Video
    {
        get => _mVideo;
        set => _mVideo = value;
    }

    public ModelData Parent
    {
        get => _mParent;
        set => _mParent = value;
    }

    public Vector3 Size
    {
        get => _mSize;
        set => _mSize = value;
    }

    public Vector3 Center
    {
        get => _mCenter;
        set => _mCenter = value;
    }

    public List<ModelData> SubModels
    {
        get => _mSubModels;
        set => _mSubModels = value;
    }

    public ModelData()
    {
        _mName = "";
        _mDescription = "";
        _mUrl = null;
        _mSubModels = new List<ModelData>();
    }
}


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
