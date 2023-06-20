using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SubModelHighlighter : MonoBehaviour
{
    private Renderer _mRenderer;
    private bool _mIsHighlighted = false;

    public bool IsHighlighted => _mIsHighlighted;

    void Awake()
    {
        _mRenderer = GetComponent<Renderer>();
    }

    public void Highlight()
    {
        _mIsHighlighted = true;
        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
            r.material.color = Color.red;
    }

    public void UnHighlight()
    {
        _mIsHighlighted = false;
        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
            r.material.color = Color.white;
    }
}