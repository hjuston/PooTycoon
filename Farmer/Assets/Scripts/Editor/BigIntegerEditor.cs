using UnityEngine;
using System.Collections;
using UnityEditor;

//[CustomEditor(typeof(BigInteger))]
public class BigIntegerEditor : Editor
{
    private SerializedObject testClass;
    private SerializedProperty testProp;

    void OnEnable()
    {
        testClass = new SerializedObject(target);
        testProp = testClass.FindProperty("Value");
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        testClass.Update();
        
        if(testProp == null)
        {
            EditorGUILayout.LabelField("Jest nullem");
        }
        else
        {
            EditorGUILayout.LabelField("Nie jest nullem");
        }

        testProp.stringValue = EditorGUILayout.TextField("Value", testProp == null ? "Gówno" : testProp.stringValue);
        
        testClass.ApplyModifiedProperties();
    }

}
