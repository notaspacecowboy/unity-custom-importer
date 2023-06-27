using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SubModelHighlighter : MonoBehaviour
{
    private Renderer _mRenderer;
    private bool _mIsHighlighted = false;
    private bool _mIsSelected = false;

    public bool IsHighlighted => _mIsHighlighted;
    public bool IsSelected => _mIsSelected;

    void Awake()
    {
        _mRenderer = GetComponent<Renderer>();
    }

    public void Highlight()
    {
        _mIsHighlighted = true;
        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
            r.material.color = new Color(1f, 0.6f, 0.6f, 0.8f);
    }

    public void UnHighlight()
    {
        _mIsHighlighted = false;
        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
            r.material.color = Color.white;
    }

    public void Selected()
    {
        _mIsSelected = true;
        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
            r.material.color = Color.white;
    }
    public void UnSelected()
    {
        _mIsSelected = false;
        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
            r.material.color = new Color(0.6f, 0.6f, 0.6f, 0.8f);
    }
}