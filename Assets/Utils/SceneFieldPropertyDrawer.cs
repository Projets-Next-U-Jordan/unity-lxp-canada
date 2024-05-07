using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");
        SerializedProperty sceneNameProp = property.FindPropertyRelative("sceneName");

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, sceneAssetProp, GUIContent.none);

        if (EditorGUI.EndChangeCheck())
        {
            if (sceneAssetProp.objectReferenceValue != null)
            {
                SceneAsset sceneAsset = sceneAssetProp.objectReferenceValue as SceneAsset;
                sceneNameProp.stringValue = sceneAsset.name;
            }
        }

        EditorGUI.EndProperty();
    }
}
