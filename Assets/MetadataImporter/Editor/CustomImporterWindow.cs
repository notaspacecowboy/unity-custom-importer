using System.Collections;
using UnityEngine;
using UnityEditor;
using System.IO;
using Codice.CM.Common;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine.Events;
using NUnit.Framework;
using UnityEditor.VersionControl;


public class CustomImporterWindow : EditorWindow
{
    private StateMachine m_stateMachine;


    [MenuItem("Metadata Importer/Importer Window")]
    public static void ShowWindow()
    {
        GetWindow<CustomImporterWindow>("Metadata Importer");
    }


    void OnEnable()
    {
        GUI.backgroundColor = EditorStylesHelper.BackGroundColor;

        m_stateMachine = new StateMachine();
        SelectModelState state = new SelectModelState(this, m_stateMachine);
        m_stateMachine.ChangeState(state);
    }

    void OnGUI()
    {
        m_stateMachine.Update();
    }

    void OnDestroy()
    {
        m_stateMachine.Clear();
    }
}