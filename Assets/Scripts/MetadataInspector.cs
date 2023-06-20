using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetadataInspector : MonoBehaviour
{
    public ModelRef _mModelRef;
    ModelData _mRoot;
    ModelData _mCurrentRoot;

    void Start()
    {
        _mRoot = _mModelRef.Root;
        _mCurrentRoot = _mRoot;

        var collider = gameObject.AddComponent<BoxCollider>();
        collider.size = _mRoot.Size;
        collider.center = _mRoot.Center;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        SubModelHighlighter currentHighlighter = null;
        if (Physics.Raycast(ray, out hit))
        {
            currentHighlighter = hit.transform.GetComponent<SubModelHighlighter>();
            if (currentHighlighter != null)
                currentHighlighter.Highlight();
        }

        foreach (SubModelHighlighter highlighter in gameObject.GetComponentsInChildren<SubModelHighlighter>())
            if (highlighter != currentHighlighter)
                highlighter.UnHighlight();
    }
}
