using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DataManager dataManager = (DataManager)target;

        base.OnInspectorGUI();
        if (GUILayout.Button("Create"))
        {
            dataManager.CreateSaveFile();
        }
        if (GUILayout.Button("Save"))
        {
          //  dataManager.CallSavingEvent();
            dataManager.Save();
        }
        if (GUILayout.Button("Load"))
        {
           // dataManager.CallLoadingEvent();
            dataManager.Load();
        }
    }
  
}
#endif
