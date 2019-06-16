using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using GOAP.Framework;
namespace GOAP.Helper
{
    partial class EditorHelper
    {
        public static void ShowPEMethodField(PEMethod method, Action<PEMethod> callback)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.Label(method.name);
            method.myOperator.symbol = OperatorPopup(method.myOperator.symbol, OperatorHelper.GetCheckOperatorForType(typeof(bool)), GUILayout.ExpandWidth(true));
            method.value = EditorGUILayout.Toggle(method.value);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                if (callback != null)
                    callback(method);
                return;
            }
            GUILayout.EndHorizontal();

            if(method.isValid)
            {
                foreach (var item in method.arguments)
                {
                    GUILayout.BeginHorizontal();

                    if (item == null)
                    {
                        EditorGUILayout.LabelField(item.name, "Non set variable");
                        break;
                    }

                    GUILayout.Label(item.variableType.FriendlyName(), GUILayout.ExpandWidth(false));

                    if (!item.useWorldState)
                    {
                        item.value = GenericField(item.name, item.value, item.variableType, method.methodReference.methodInfo);
                    }
                    else
                    {
                        List<string> variableNames = new List<string>();

                        if (method.worldState != null)
                        {
                            variableNames.AddRange(method.worldState.GetVariableNames(item.variableType));
                        }
                        item.name = StringPopup("", item.name, variableNames);
                    }

                    item.useWorldState = EditorGUILayout.Toggle(item.useWorldState, EditorStyles.radioButton, GUILayout.Width(15));


                    GUILayout.EndHorizontal();

                }
            }
      

            GUILayout.EndVertical();

        }

        public static void ShowPEParameterFieldPrecondition(PEParameter parameter, Action<PEParameter> callback)
        {

            GUILayout.BeginHorizontal();
            GUILayout.Label(parameter.name);
            if (!parameter.isValid)
            {
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    if (callback != null)
                        callback(parameter);
                    return;
                }
                GUILayout.EndHorizontal();

                return;
            }
            parameter.myOperator.symbol = OperatorPopup(parameter.myOperator.symbol, OperatorHelper.GetCheckOperatorForType(parameter.variableReference.myType), GUILayout.ExpandWidth(true));
            parameter.value = GenericField(parameter.value, parameter.variableType, null);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                if (callback != null)
                    callback(parameter);
                return;
            }
            GUILayout.EndHorizontal();
        }
        public static void ShowPEParameterFieldEffect(PEParameter parameter, Action<PEParameter> callback)
        {

            GUILayout.BeginHorizontal();
            GUILayout.Label(parameter.name);
            if (!parameter.isValid)
            {
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    if (callback != null)
                        callback(parameter);
                    return;
                }
                GUILayout.EndHorizontal();

                return;
            }
            parameter.myOperator.symbol = OperatorPopup(parameter.myOperator.symbol, OperatorHelper.GetSetOperatorForType(parameter.variableReference.myType), GUILayout.ExpandWidth(true));
            parameter.value = GenericField(parameter.value, parameter.variableType, null);

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                if (callback != null)
                    callback(parameter);
                return;
            }
            GUILayout.EndHorizontal();
        }
        public static string OperatorPopup(string selected, List<string> operators, params GUILayoutOption[] guiOption)
        {
            if (operators == null)
                return selected;

            int index = 0;
            var copy = new List<string>(operators);

            if (copy.Contains(selected))
                index = copy.IndexOf(selected);
            else
                index = 0;

            index = EditorGUILayout.Popup(index, copy.ToArray(), guiOption);
            return copy[index];
        }
    }
}