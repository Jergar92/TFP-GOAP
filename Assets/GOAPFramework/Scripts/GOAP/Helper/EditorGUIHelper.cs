using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEditor;
using GOAP.Framework;
namespace GOAP.Helper
{
    partial class EditorHelper
    {
        private static readonly float separatorHeight = 2;
        public static void Separator()
        {
            GUI.backgroundColor = Color.black;
            GUILayout.Box("", GUILayout.MaxWidth(Screen.width), GUILayout.Height(separatorHeight));
            GUI.backgroundColor = Color.white;
        }
        public static void Separator(Color color)
        {
            GUI.backgroundColor = color;
            GUILayout.Box("", GUILayout.MaxWidth(Screen.width), GUILayout.Height(separatorHeight));
            GUI.backgroundColor = Color.white;
        }
        public static void ShowAutomaticEditor(object obj)
        {
            if (obj == null)
                return;

            foreach (var field in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                field.SetValue(obj, GenericField(field.Name, field.GetValue(obj), field.FieldType, field, obj));
                GUI.backgroundColor = Color.white;
            }
            GUI.enabled = Application.isPlaying;
            foreach (var property in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (property.CanRead && property.CanWrite)
                {
                    if (property.DeclaringType.GetField("<" + property.Name + ">__BackingField", BindingFlags.NonPublic | BindingFlags.Instance) != null)
                    {
                        GenericField(property.Name, property.GetValue(obj, null), property.PropertyType, property, obj);
                    }
                }
            }
            GUI.enabled = true;
        }
        public static object GenericField(object fieldValue, Type fieldType, MemberInfo member, object instance = null)
        {
            if (fieldType == null)
            {
                GUILayout.Label("ERROR: NO Type");
                return fieldValue;
            }
            if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
            {
                if (fieldType == typeof(Component) && (Component)fieldValue != null)
                {
                }
                return EditorGUILayout.ObjectField((UnityEngine.Object)fieldValue, fieldType, true);
            }

            if (fieldType == typeof(bool))
                return EditorGUILayout.Toggle((bool)fieldValue);

            if (fieldType == typeof(int))
                return EditorGUILayout.IntField((int)fieldValue);

            if (fieldType == typeof(float))
                return EditorGUILayout.FloatField((float)fieldValue);

            if (fieldType == typeof(string))
                return EditorGUILayout.TextField((string)fieldValue);

            if (fieldType == typeof(Vector2))
                return EditorGUILayout.Vector2Field("", (Vector2)fieldValue);

            if (fieldType == typeof(Vector3))
                return EditorGUILayout.Vector3Field("", (Vector3)fieldValue);

            if (fieldType == typeof(Vector4))
                return EditorGUILayout.Vector4Field("", (Vector4)fieldValue);

            return null;
        }

        public static object GenericField(string fieldName, object fieldValue, Type fieldType, MemberInfo member, object instance = null)
        {
            if (fieldType == null)
            {
                GUILayout.Label("ERROR: NO Type");
                return fieldValue;
            }
            IEnumerable<Attribute> attributes = new Attribute[0];
            if (member != null)
            {
                if (fieldType.GetCustomAttributes(typeof(HideInInspector), true).FirstOrDefault() != null)
                {
                    return fieldValue;
                }
                attributes = member.GetCustomAttributes(true).Cast<Attribute>();

                if (attributes.Any(a => a is HideInInspector))
                {
                    return fieldValue;
                }

                NameAttribute name_attribute = attributes.FirstOrDefault(a => a is NameAttribute) as NameAttribute;
            }
            if (typeof(WSParameter).IsAssignableFrom(fieldType))
            {
                return WSParameterField(fieldName, (WSParameter)fieldValue, false, member);
            }
            if (typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
            {
                if (fieldType == typeof(Component) && (Component)fieldValue != null)
                {
                }
                return EditorGUILayout.ObjectField(fieldName, (UnityEngine.Object)fieldValue, fieldType, true);
            }
            if (fieldType == typeof(bool))
                return EditorGUILayout.Toggle(fieldName, (bool)fieldValue);

            if (fieldType == typeof(int))
                return EditorGUILayout.IntField(fieldName, (int)fieldValue);

            if (fieldType == typeof(float))
                return EditorGUILayout.FloatField(fieldName, (float)fieldValue);

            if (fieldType == typeof(string))
                return EditorGUILayout.TextField(fieldName, (string)fieldValue);

            if (fieldType == typeof(Vector2))
                return EditorGUILayout.Vector2Field(fieldName, (Vector2)fieldValue);

            if (fieldType == typeof(Vector3))
                return EditorGUILayout.Vector3Field(fieldName, (Vector3)fieldValue);

            if (fieldType == typeof(Vector4))
                return EditorGUILayout.Vector4Field(fieldName, (Vector4)fieldValue);


            return null;
        }

        public static string StringPopup(string prefix, string selected, List<string> options)
        {
            EditorGUILayout.BeginVertical();

            List<string> copyOptions = new List<string>(options);
            int index = 0;

            copyOptions.Insert(0, "|NONE|");
            if (copyOptions.Contains(selected))
                index = copyOptions.IndexOf(selected);
            else
            {
                index = 0;
            }
            if (!string.IsNullOrEmpty(prefix))
                index = EditorGUILayout.Popup(prefix, index, copyOptions.ToArray());
            else
            {
                index = EditorGUILayout.Popup(index, copyOptions.ToArray());
            }
            EditorGUILayout.EndVertical();
            return index == 0 ? string.Empty : copyOptions[index];
        }
    }
}