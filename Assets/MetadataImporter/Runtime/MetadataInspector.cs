using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetadataInspector : MonoSingleton<MetadataInspector>
{
    private List<MetadataComponent> m_components;
    private SubModelHighlighter m_selected;

    public void Add(MetadataComponent component)
    {
        if (!m_components.Contains(component))
            m_components.Add(component);
    }
    public void Remove(MetadataComponent component)
    {
        if (m_components.Contains(component))
            m_components.Remove(component);
    }

    public void Clear()
    {
        m_components.Clear(); 
    }



    #region lifecycle APIs

    private void Awake()
    {
        m_components = new List<MetadataComponent>();
        m_selected = null;
    }

    private void Update()
    {
        ProcessInput();
    }

    #endregion


    private void ProcessInput()
    {
        if(!Input.GetMouseButtonDown(0))
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hit))
        {
            if (m_selected != null)
            {
                m_selected.Select(false);
                m_selected = null;
            }
            return;
        }

        SubModelHighlighter highlighter = hit.transform.GetComponent<SubModelHighlighter>();
        if (highlighter == null)
        {
            if (m_selected != null)
            {
                m_selected.Select(false);
                m_selected = null;
            }
            return;
        }

        if (m_selected != null)
            m_selected.Select(false);
        highlighter.Select(true);
        m_selected = highlighter;
    }
}
