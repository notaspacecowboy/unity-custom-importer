using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class SubModelHighlighter : MonoBehaviour
{
    private List<Renderer> m_renderers;
    private bool m_isSelected;

    private int m_originalLayer;
    private int m_selectLayer;

    //private bool m_isHighlighted;
    //private int m_highlightLayer;

    public bool IsSelected => m_isSelected;

    public UnityAction<SubModelHighlighter> OnSelected;
    public UnityAction<SubModelHighlighter> OnUnselected;

    void Awake()
    {
        m_renderers = new List<Renderer>();
        GetComponentsInChildren<Renderer>(m_renderers);
        m_isSelected = false;

        m_originalLayer = gameObject.layer;
        m_selectLayer = LayerMask.NameToLayer("Select");


        //m_isHighlighted = false;
        //m_highlightLayer = LayerMask.NameToLayer("Hover");
    }

    public void Select(bool isSelected)
    {
        m_isSelected = isSelected;
        if (isSelected)
        { 
            foreach (var renderer in m_renderers)
                renderer.gameObject.layer = m_selectLayer;

            OnSelected?.Invoke(this);
        }
        else
        {
            foreach (var renderer in m_renderers)
                renderer.gameObject.layer = m_originalLayer;

            OnUnselected?.Invoke(this);
        }
    }


    //private void OnMouseEnter()
    //{
    //    if (m_isSelected)
    //        return;

    //    foreach(var renderer in m_renderers)
    //        renderer.gameObject.layer = m_highlightLayer;
    //}

    //private void OnMouseExit()
    //{
    //    if (m_isSelected)
    //        return;

    //    foreach (var renderer in m_renderers)
    //        renderer.gameObject.layer = m_originalLayer;
    //}
    
}