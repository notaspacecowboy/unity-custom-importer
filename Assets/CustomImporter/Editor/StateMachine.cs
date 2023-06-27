using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class IImportWindowState
{
    private EditorWindow m_editorWindow;
    
    private StateMachine m_owner;
    
    protected float m_minHorizontalSpace;
    protected ModelRef m_existingProfile;
    protected float m_currentWidth;
    protected float m_currentHeight;
    protected float m_extraHorSpace;

    private static float m_minWidth;
    private static float m_minHeight;

    protected EditorWindow EditorWindow => m_editorWindow;
    protected StateMachine Owner => m_owner;

    protected float MinWidth
    {
        get => EditorWindow.minSize.x;
        set
        {
            m_minWidth = value;
            EditorWindow.minSize = new Vector2(m_minWidth, m_minHeight);
        }
    }

    protected float MinHeight
    {
        get => EditorWindow.minSize.y;
        set
        {
            m_minHeight = value;
            EditorWindow.minSize = new Vector2(m_minWidth, m_minHeight);
        }
    }


    public IImportWindowState(EditorWindow window, StateMachine owner, float minWidth = 500, float minHeight = 500, float minHorSpace = 300)
    {
        m_editorWindow = window;
        m_owner = owner;

        m_minWidth = minWidth;
        m_minHeight = minHeight;
        m_minHorizontalSpace = minHorSpace;
        m_currentWidth = m_editorWindow.position.width;
        m_currentHeight = m_editorWindow.position.height;

        m_extraHorSpace = m_currentWidth - MinWidth;
        EditorWindow.minSize = new Vector2(m_minWidth, m_minHeight);

        m_extraHorSpace = 0;
    }

    public virtual void Update()
    {
        m_currentWidth = EditorWindow.position.width;
        m_currentHeight = EditorWindow.position.height;
        m_extraHorSpace = m_currentWidth - MinWidth;

        EditorWindow.minSize = new Vector2(m_minWidth, m_minHeight);
    }

    public void ChangeState(IImportWindowState state)
    {
        m_owner.ChangeState(state);
    }
}

public class StateMachine
{
    private IImportWindowState _mCurrentState;

    public void Update()
    {
        _mCurrentState?.Update();
    }
    public void ChangeState(IImportWindowState state)
    {
        _mCurrentState = state;
    }
}
