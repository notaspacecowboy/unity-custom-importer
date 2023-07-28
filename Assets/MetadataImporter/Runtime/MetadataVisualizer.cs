using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MetadataVisualizer : MonoSingleton<MetadataVisualizer>, IStringFieldVisualizer, IImageFieldVisualizer, IVideoFieldVisualizer
{
    [SerializeField] 
    private GameObject m_panel;

    [SerializeField] 
    private TextMeshProUGUI m_fieldName;
    
    [SerializeField]
    private TextMeshProUGUI m_stringField;

    [SerializeField]
    private Camera m_mainCamera;

    [SerializeField]
    public Camera m_inspectCamera;


    private bool m_isEnabled;
    public bool Enabled
    {
        get => m_isEnabled;
        set
        {
            if (!value)
            {
                Hide();
                m_currentData = null;
            }
            m_isEnabled = value;
        }
    }

    public TextMeshProUGUI StringField
    {
        get => m_stringField;
    }

    [SerializeField]
    private Image m_imageField;

    public Image ImageField
    {
        get => m_imageField;
    }

    [SerializeField]
    private VideoPlayer m_videoField;

    public VideoPlayer VideoField
    {
        get => m_videoField;
    }


    private ModelData m_currentData;
    private int m_currentIndex;

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

    public void Show(ModelData data)
    {
        m_panel.gameObject.SetActive(true); //for now
        m_currentIndex = 0;
        m_currentData = data;
        ShowField(data, data.MetadataList[0]);
    }

    public void Hide()
    {
        m_panel.gameObject.SetActive(false);
    }

    private void ShowField(ModelData data, FieldData field)
    {
        m_stringField.gameObject.SetActive(false);
        m_imageField.gameObject.SetActive(false);
        m_videoField.gameObject.SetActive(false);

        m_fieldName.text = $"{data.Name}\n{field.FieldName}";
        field.ShowField(this);
    }

    #region lifecycle APIs

    private void Awake()
    {
        m_components = new List<MetadataComponent>();
    }

    private void Update()
    {
        if (!m_isEnabled || m_panel.gameObject.activeSelf)
            return;

        ProcessInput();
    }

    #endregion


    private void ProcessInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (m_selected == null)
                return;

            m_selected.Select(false);
            if (m_selected.ModelData.Parent == null)
            {
                m_selected = null;
                return;
            }

            var parent = m_selected.ModelData.Parent;    //omg this is so bad, should find a better way to do this
            m_selected = parent.Transform.GetComponent<SubModelHighlighter>();
            m_selected.Select(true);
        }
        else if (Input.GetMouseButtonDown(0))
        {
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

    public void OnCloseButtonClicked()
    {
        m_panel.gameObject.SetActive(false);
        m_currentData = null;
    }

    public void OnNextButtonClicked()
    {
        m_currentIndex++;
        if (m_currentIndex >= m_currentData.MetadataList.Count)
            m_currentIndex = 0;
        ShowField(m_currentData, m_currentData.MetadataList[m_currentIndex]);
    }

    public void OnPreviousButtonClicked()
    {
        m_currentIndex--;
        if (m_currentIndex < 0)
            m_currentIndex = m_currentData.MetadataList.Count - 1;
        ShowField(m_currentData, m_currentData.MetadataList[m_currentIndex]);
    }

    public void OnSwitchViewButtonClicked()
    {
        m_mainCamera?.gameObject.SetActive(!m_mainCamera.gameObject.activeSelf);
        m_inspectCamera?.gameObject.SetActive(!m_inspectCamera.gameObject.activeSelf);

        m_panel.gameObject.SetActive(false);
        m_currentData = null;
    }
}
