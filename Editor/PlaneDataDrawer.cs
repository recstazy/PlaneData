using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace Recstazy.PlaneData.Editor
{
    [CustomPropertyDrawer(typeof(PlaneDataAttribute), true)]
    public class PlaneDataDrawer : PropertyDrawer
    {
        #region Fields

        private SerializedProperty _property;
        private Rect _rect;
        private GUIContent _label;
        private static readonly float s_buttonSize = EditorGUIUtility.singleLineHeight - EditorGUIUtility.standardVerticalSpacing;
        private static readonly GUIStyle s_buttonStyle = new GUIStyle(GUI.skin.button);
        private static readonly RectOffset s_buttonPadding = new RectOffset(0, 0, 1, 2);
        private static readonly string[] SelectTypeCaption = new string[] { "- Select Type -" };

        #endregion

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
                {
                    return EditorGUIUtility.singleLineHeight;
                }
            }

            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                _rect = position;
                _property = property;
                _label = label;

                if (string.IsNullOrEmpty(property.managedReferenceFullTypename))
                {
                    DrawCreateMode();
                }
                else
                {
                    DrawProperty();
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        private void DrawCreateMode()
        {
            Rect rect = _rect;
            rect.width = EditorGUIUtility.labelWidth;
            EditorGUI.LabelField(rect, _label);

            rect.x += rect.width;
            rect.width = _rect.width - EditorGUIUtility.labelWidth;

            var fieldType = GetDeclaredFieldType();
            IList<Type> types = TypeCache.GetTypesDerivedFrom(fieldType);

            if (!fieldType.IsInterface && !fieldType.IsAbstract)
            {
                types = new Type[] { fieldType }.Concat(types).ToArray();
            }

            var names = SelectTypeCaption
                .Concat(types.Select(t => t.Name))
                .ToArray();
            
            int index = EditorGUI.Popup(rect, 0, names);

            if (index != 0)
            {
                var instance = Activator.CreateInstance(types[index - 1]);
                _property.managedReferenceValue = instance;
            }
        }

        private void DrawProperty()
        {
            Rect rect = _rect;
            rect.width -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            string name = _property.managedReferenceFullTypename;
            int indexOfName;

            if (name.Contains('.'))
            {
                indexOfName = name.LastIndexOf('.') + 1;
            }
            else
            {
                indexOfName = name.LastIndexOf(' ') + 1;
            }

            name = ObjectNames.NicifyVariableName(name.Substring(indexOfName, name.Length - indexOfName));

            if (IsArrayOrList())
            {
                if (_label.text.Contains("Element "))
                {
                    name = $"{_label.text.Replace("Element ", "")} - {name}"; 
                }
            }

            EditorGUI.PropertyField(rect, _property, new GUIContent(name), true);

            rect.x = _rect.x + rect.width + EditorGUIUtility.standardVerticalSpacing;
            rect.width = s_buttonSize;
            rect.height = s_buttonSize;
            s_buttonStyle.padding = s_buttonPadding;
            s_buttonStyle.alignment = TextAnchor.MiddleCenter;
            s_buttonStyle.fontSize = 12;

            if (GUI.Button(rect, "x", s_buttonStyle))
            {
                _property.managedReferenceValue = null;
            }
        }

        private Type GetDeclaredFieldType()
        {
            var fieldType = fieldInfo.FieldType;

            if (IsArrayOrList())
            {
                if (fieldType.IsArray)
                {
                    return fieldType.GetElementType();
                }
                else if (fieldType.IsGenericType)
                {
                    return fieldType.GetGenericArguments()[0];
                }
            }

            return fieldInfo.FieldType;
        }

        private bool IsArrayOrList()
        {
            return typeof(IList).IsAssignableFrom(fieldInfo.FieldType);
        }
    }
}
