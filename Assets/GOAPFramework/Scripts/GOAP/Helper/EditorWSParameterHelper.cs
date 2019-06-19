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

        public static WSParameter WSParameterField(string fieldName, WSParameter parameter, bool worldStateOnly = false, MemberInfo member = null)
        {
            if (parameter == null)
            {
                EditorGUILayout.LabelField(fieldName, "Non set variable");
                return null;
            }
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (!worldStateOnly && !parameter.useWorldState)
            {
                GUILayout.BeginVertical();
                parameter.value = GenericField(fieldName, parameter.value, parameter.variableType, member);
                GUILayout.EndVertical();
            }
            else
            {
                List<string> variableNames = new List<string>();

                if (parameter.worldState != null)
                {
                    variableNames.AddRange(parameter.worldState.GetVariableNames(parameter.variableType));
                }
                parameter.name = StringPopup(fieldName, parameter.name, variableNames);
            }

            parameter.useWorldState = EditorGUILayout.Toggle(parameter.useWorldState, EditorStyles.radioButton, GUILayout.Width(15));


            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            return parameter;
        }
    }
}