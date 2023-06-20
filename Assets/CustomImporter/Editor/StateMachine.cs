using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class IImportWindowState
{
    private EditorWindow _mEditorWindow;
    
    private StateMachine _mOwner;
    protected EditorWindow EditorWindow => _mEditorWindow;

    protected StateMachine Owner => _mOwner;

    public IImportWindowState(EditorWindow window, StateMachine owner)
    {
        _mEditorWindow = window;
        _mOwner = owner;
    }
    public abstract void Update();

    public void ChangeState(IImportWindowState state)
    {
        _mOwner.ChangeState(state);
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
