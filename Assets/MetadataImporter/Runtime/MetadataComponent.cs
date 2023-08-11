using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MetadataComponent : MonoBehaviour
{
    [SerializeField]
    private ModelRef m_modelRef;
    public ModelRef ModelRef
    {
        get
        {
            if (m_modelRef == null)
                m_modelRef = GetComponent<ModelRef>();
            return m_modelRef;
        }
        set => m_modelRef = value;
    }


    private ModelData m_root;
    private ModelData m_currentRoot;
    
    private void Awake()
    {
        if (m_modelRef == null)
        {
            Debug.LogError("ModelRef is invalid");
            return;
        }
        
        m_root                = m_modelRef.Root;
        m_currentRoot         = null;
        m_modelRef.GameObject = this.gameObject;

        var highlighters = GetComponentsInChildren<SubModelHighlighter>();
        highlighters.ToList().ForEach(x =>
        {
            x.OnSelected += OnSubModelSelected;
            x.OnUnselected += OnSubModelUnselected;
        });
    }

    private void Start()
    {
        FixModelDataReference(ModelRef.Root, this.transform);
        EnableCollider(m_root);
    }


    private void FixModelDataReference(ModelData rootData, Transform root)
    {
        rootData.Transform = root;
        rootData.Collider = rootData.Transform.GetComponent<BoxCollider>();

        var highlighter = rootData.Transform.GetComponent<SubModelHighlighter>();
        highlighter.ModelData = rootData;

        for (int i = 0; i < rootData.SubModels.Count; i++)
        {
            rootData.SubModels[i].Parent = rootData;
            FixModelDataReference(rootData.SubModels[i], transform.GetChild(rootData.SubModels[i].Index));
        }
    }

    private void EnableCollider(ModelData model)
    {
        model.Collider.enabled = true;
    }

    private void DisableCollider(ModelData model)
    {
        model.Collider.enabled = false;
    }

    private void OnSubModelSelected(SubModelHighlighter highlighter)
    {
        //find model data attached to the selected highlighter
        if (m_currentRoot == null) 
            DisableCollider(m_root);
        else
        {
            foreach (var model in m_currentRoot.SubModels)
                DisableCollider(model);
        }

        m_currentRoot = highlighter.ModelData;

        foreach (var model in m_currentRoot.SubModels)
            EnableCollider(model);
        

        MetadataVisualizer.Instance.Show(m_currentRoot, m_modelRef);
    }


    private void OnSubModelUnselected(SubModelHighlighter highlighter)
    {
        foreach (var model in m_currentRoot.SubModels)
            DisableCollider(model);

        m_currentRoot = null;
        EnableCollider(m_root);
    }
}
