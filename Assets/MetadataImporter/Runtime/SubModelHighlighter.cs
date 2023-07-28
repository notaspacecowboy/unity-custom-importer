using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class SubModelHighlighter : MonoBehaviour
{
    public ModelData ModelData { get; set; }


    private List<Renderer> m_renderers;
    private bool m_isSelected;

    private int m_originalLayer;
    private int m_selectLayer;

    //private bool m_isHighlighted;
    //private int m_highlightLayer;

    private Dictionary<Renderer, Material[]> m_originalMaterials;

    public bool IsSelected => m_isSelected;

    public UnityAction<SubModelHighlighter> OnSelected;
    public UnityAction<SubModelHighlighter> OnUnselected;

    void Awake()
    {
        m_renderers = new List<Renderer>();
    }

    void Start()
    {
        GetComponentsInChildren<Renderer>(m_renderers);
        m_isSelected = false;
        m_originalLayer = gameObject.layer;
        m_selectLayer = LayerMask.NameToLayer("Select");
    }

    public void Select(bool isSelected)
    {
        //should not be selected twice
        if (m_isSelected == isSelected)
        {
            Debug.LogError($"{this.gameObject.name} got {(isSelected? "selected" : "unselected")} twice, this is not allowed!");
            return;
        }

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

    public void ChangeColor(Color newColor)
    {
        foreach (var renderer in m_renderers)
            renderer.material.color = newColor;
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