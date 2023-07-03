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

        //create a new template
        MetadataTemplate newTemplate = new MetadataTemplate();
        newTemplate.Name = templateName;

        // Read the file line by line to find all fields
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
            else if (newTemplate.Fields.Exists(field => field.Name == fieldName))
            {
                Debug.LogError("field name already exists");
                return;
            }
            bool isParaData = (row[2] == "yes" ? true : false);

            newTemplate.Fields.Add(new Field() { Name = fieldName, Type = fieldType, IsParaData = isParaData });
        }

        m_metadataTemplates.Templates.Add(newTemplate);
    }
}
