using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Attributes
{ 

    public class ConditionalHideCondition
    {
        public string FieldName;
        public object Value;

        public ConditionalHideCondition(string fieldName, object value)
        {
            this.FieldName = fieldName;
            this.Value = value;
        }
    }

    public class ConditionalHideAttribute : PropertyAttribute
    {
        public string[] Conditions;
        public bool HideInInspector = false;

        public ConditionalHideAttribute(bool hideInInspector, params string[] conditions)
        {
            this.Conditions = conditions;
            this.HideInInspector = hideInInspector;
        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHidePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;
            if (!condHAtt.HideInInspector || enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }

            GUI.enabled = wasEnabled;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
            bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

            if (!condHAtt.HideInInspector || enabled)
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            else
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
        {
            bool enabled = true;

            foreach (var condition in condHAtt.Conditions)
            {
                string[] parts = condition.Split('=');
                if (parts.Length != 2)
                {
                    Debug.LogWarning("Invalid condition: " + condition);
                    continue;
                }

                string fieldName = parts[0].Trim();
                string valueStr = parts[1].Trim();

                string propertyPath = property.propertyPath;
                string conditionPath = propertyPath.Replace(property.name, fieldName);
                SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

                if (sourcePropertyValue != null)
                {
                    if (sourcePropertyValue.propertyType == SerializedPropertyType.Boolean)
                        enabled &= sourcePropertyValue.boolValue.Equals(bool.Parse(valueStr));
                    else if (sourcePropertyValue.propertyType == SerializedPropertyType.Integer)
                        enabled &= sourcePropertyValue.intValue.Equals(int.Parse(valueStr));
                    else if (sourcePropertyValue.propertyType == SerializedPropertyType.Enum)
                        enabled &= sourcePropertyValue.enumValueIndex.Equals(int.Parse(valueStr));
                    else if (sourcePropertyValue.propertyType == SerializedPropertyType.Float)
                        enabled &= sourcePropertyValue.floatValue.Equals(float.Parse(valueStr));
                    else if (sourcePropertyValue.propertyType == SerializedPropertyType.String)
                        enabled &= sourcePropertyValue.stringValue.Equals(valueStr);
                }
                else
                {
                    Debug.LogWarning("Attempting to use a ConditionalHideAttribute but no matching SourcePropertyValue found in object: " + fieldName);
                }

                if (!enabled) break;
            }

            return enabled;
        }
    }   
    #endif
}