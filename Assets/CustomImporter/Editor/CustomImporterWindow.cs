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
    private StateMachine _mStateMachine;


    [MenuItem("Window/Custom Importer")]
    public static void ShowWindow()
    {
        GetWindow<CustomImporterWindow>("Custom Importer");
    }


    void OnEnable()
    {
        GUI.backgroundColor = EditorStylesHelper.BackGroundColor;

        _mStateMachine = new StateMachine();
        SelectModelState state = new SelectModelState(this, _mStateMachine);
        _mStateMachine.ChangeState(state);
    }

    void OnGUI()
    {
        _mStateMachine.Update();
    }
}