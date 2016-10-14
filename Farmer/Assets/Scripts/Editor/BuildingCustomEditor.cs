using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Building))]
public class BuildingCustomEditor : Editor {

    private SerializedObject testClass;
    private SerializedProperty testProp;

    void OnEnable()
    {
        testClass = new SerializedObject(target);
        testProp = testClass.FindProperty("_maximumShits");
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        testClass.Update();
        Building b = (Building)target;

        b.MaximumShits = new BigInteger(EditorGUILayout.TextField("Max. shits", b.MaximumShits == null ? "" : b.MaximumShits.ToString()));
        b.CurrentShits = new BigInteger(EditorGUILayout.TextField("Curr. shits", b.CurrentShits == null ? "" : b.CurrentShits.ToString()));

        testClass.ApplyModifiedProperties();
    }
}
