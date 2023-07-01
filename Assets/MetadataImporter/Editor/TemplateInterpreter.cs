using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class TemplateImporter: Singleton<TemplateImporter>
{
    private MetadataTemplates m_metadataTemplates;

    public MetadataTemplates Template => m_metadataTemplates;

    protected override void Init()
    {
        base.Init();

        string[] guids = AssetDatabase.FindAssets("t:MetadataTemplates");
        if (guids.Length == 0)
        {
            Debug.LogError("MetadataTemplates not found");
            return;
        }
        else if (guids.Length > 1)
        {
            Debug.LogError("Multiple MetadataTemplates found");
            return;
        }

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        m_metadataTemplates = AssetDatabase.LoadAssetAtPath<MetadataTemplates>(path);
    }

    public void Import(string csvFilePath)
    {
        string templateName = Path.GetFileNameWithoutExtension(csvFilePath);
        foreach (var template in m_metadataTemplates.Templates)
        {
            if (templateName == template.Name)
            {
                m_metadataTemplates.Templates.Remove(template);
                break;
            }
        }

        Dictionary<string, FieldType> fields = new Dictionary<string, FieldType>();

        // Read the file line by line
        foreach (string line in File.ReadLines(csvFilePath).Skip(1))
        {
            var row = line.Split(',');
            var fieldName = row[0];
            FieldType fieldType;
            if(!Enum.TryParse(row[1], true, out fieldType))
            {
                Debug.LogError("field type not recognizable");
                return;
            }
            else if (fields.ContainsKey(fieldName))
            {
                Debug.LogError("field name already exists");
                return;
            }

            fields.Add(fieldName, fieldType);
        }

        GenerateMetadataTemplate(templateName, fields);
    }

    private void GenerateMetadataTemplate(string templateName, Dictionary<string, FieldType> fields)
    {
        MetadataTemplate template = new MetadataTemplate();
        template.Name = templateName;
        template.Fields = new List<Field>();
        foreach (var field in fields)
            template.Fields.Add(new Field() { Name = field.Key, Type = field.Value });

        m_metadataTemplates.Templates.Add(template);
    }
}
