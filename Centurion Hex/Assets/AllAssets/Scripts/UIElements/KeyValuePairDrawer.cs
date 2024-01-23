using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(KeyTypeValueTuple))]
public class KeyTypeValueTupleDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var keyRect = new Rect(position.x, position.y, position.width/2-35, position.height);
        var typeRect = new Rect(position.x + position.width/2-30, position.y, 60, position.height);
        var valueRect = new Rect(position.x + position.width/2+35, position.y, position.width/2-35, position.height);

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("key"), GUIContent.none);
        EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("type"), GUIContent.none);
        EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}