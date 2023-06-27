using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MetadataInspector : MonoBehaviour
{
    [SerializeField]
    private ModelRef _mModelRef;
    ModelData _mRoot;
    ModelData _mCurrentRoot;
    List<SubModelHighlighter> _mHighlighters = new List<SubModelHighlighter>();
    public ModelRef ModelRef
    {
        get
        {
            if (_mModelRef == null)
                _mModelRef = GetComponent<ModelRef>();
            return _mModelRef;
        }
        set => _mModelRef = value;
    }

    void Start()
    {
        _mRoot = _mModelRef.Root;
        _mCurrentRoot = _mRoot;
        FixModelDataReference(ModelRef.Root, transform);
        AddCollider(_mCurrentRoot);
        //AddHighlighter(_mCurrentRoot);
    }

    void Update()
    {
        ProcessPlayerInput();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        SubModelHighlighter currentHighlighter = null;
        if (Physics.Raycast(ray, out hit))
        {
            currentHighlighter = hit.transform.GetComponent<SubModelHighlighter>();
            if (currentHighlighter != null)
                currentHighlighter.Highlight();

            foreach (SubModelHighlighter highlighter in _mHighlighters)
                if (highlighter != currentHighlighter)
                    highlighter.UnHighlight();
        }
        else
        {
            foreach (SubModelHighlighter highlighter in _mHighlighters)
                highlighter.UnHighlight();
        }
    }

    private void ProcessPlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
        }
    }

    private void FixModelDataReference(ModelData rootData, Transform root)
    {
        rootData.Transform = root;
        for (int i = 0; i < rootData.SubModels.Count; i++)
            FixModelDataReference(rootData.SubModels[i], transform.GetChild(rootData.SubModels[i].Index));
    }

    private void AddCollider(ModelData model)
    {
        var collider = model.Transform.gameObject.AddComponent<BoxCollider>();
        collider.size = _mRoot.Size;
        collider.center = _mRoot.Center;

        var highlighter = model.Transform.AddComponent<SubModelHighlighter>();
        _mHighlighters.Add(highlighter);
    }

    private void Addhighlighter(ModelData model)
    {
        var highlighter = model.Transform.AddComponent<SubModelHighlighter>();
        highlighter.UnSelected();
        _mHighlighters.Add(highlighter);
    }
}
