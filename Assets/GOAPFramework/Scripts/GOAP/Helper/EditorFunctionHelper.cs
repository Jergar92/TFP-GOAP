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

        public static void FunctionInfo(FunctionBase function, IFunctionSystem ownerSystem, Type type, Action<FunctionBase> callback)
        {

            if (function == null)
                ShowFunctionSelect(ownerSystem, type, callback);
            else
                FunctionBase.ShowFunctionInspector(function, callback);
        }

        private static void ShowFunctionSelect(IFunctionSystem ownerSystem, Type type, Action<FunctionBase> callback)
        {
            Action<Type> FunctionTypeSelected = (t) =>
            {
                FunctionBase new_function = FunctionBase.Create(t, ownerSystem);
                callback(new_function);
            };
            Func<GenericMenu> GetFunctionMenu = () =>
            {
                GenericMenu menu = GetTypeSelectionMenu(type, FunctionTypeSelected);

                if (FunctionBase.copyFunction != null && type.IsAssignableFrom(FunctionBase.copyFunction.GetType()))
                    menu.AddItem(new GUIContent(string.Format("Paste ({0})", FunctionBase.copyFunction.name)), false, () => { callback(FunctionBase.copyFunction.Duplicate(ownerSystem)); });

                return menu;
            };
            GUI.backgroundColor = Color.red;
            GUILayout.BeginHorizontal();
            string button_text = "Assign " + type.Name;
            if (GUILayout.Button(button_text))
            {
                GenericMenu menu = GetFunctionMenu();

                menu.ShowAsContext();

                Event.current.Use();
            }
            GUILayout.EndHorizontal();

            GUI.backgroundColor = Color.white;

        }
    }
}