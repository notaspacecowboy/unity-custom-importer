using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public enum FieldType
{
    String,
    Image,
    VideoClip
}

[Serializable]
public class Field
{
    [SerializeField] 
    public string Name;
    [SerializeField]
    public FieldType Type;
}


[Serializable]
public class MetadataTemplate
{
    [SerializeField]
    public string Name;

    [SerializeField]
    public List<Field> Fields;
}


[CreateAssetMenu(fileName = "Metadata Templates", menuName = "Custom Importer/Metadata Templates", order = 1)]
[Serializable]
public class MetadataTemplates : ScriptableObject
{
    public List<MetadataTemplate> Templates;
}
