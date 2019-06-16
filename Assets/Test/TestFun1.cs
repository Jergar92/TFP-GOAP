#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using GOAP.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using GOAP.Helper;

[CustomEditor(typeof(TestFun))]
public class TestFun1 : UnityEditor.Editor
{


    private TestFun testFun
    {
        get { return (TestFun)target; }
    }
    public override void OnInspectorGUI()
    {
        if (Event.current.isMouse)
        {
            Repaint();
        }
        ShowVariables(this);

        if (Application.isPlaying)
        {
            Repaint();
        }
    }



public void ShowVariables(UnityEngine.Object parent)
{
    GUI.skin.label.richText = true;
    Event e = Event.current;
    var layout_options = new GUILayoutOption[] { GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true), GUILayout.Height(20) };


    if (GUILayout.Button("Add Variable"))
    {
    

        /*
    System.Action<PropertyInfo> selectedGOProperty = (p) =>
    {
        WSVariable new_variable = world_state.AddVariable(p.Name, p.PropertyType);
        new_variable.BindProperty(p);
    };

    System.Action<FieldInfo> selectedGOField = (f) =>
    {
        WSVariable new_variable = world_state.AddVariable(f.Name, f.FieldType);
        new_variable.BindProperty(f);
    };
    */

        System.Action<MethodInfo> selectedGOMethod = (f) =>
        {
            //WSVariable new_variable = world_state.AddVariable(f.Name, f.MethodHandle.GetType());

            //  new_variable.BindProperty(f);

            Type data_type = typeof(WSVariable<>).RHelperGenericType(new Type[] { f.ReturnType });
            testFun.method = (PEMethod)Activator.CreateInstance(data_type);
        };

        GenericMenu menu = new GenericMenu();


        menu = EditorHelper.GetMethodSelectionMenu(selectedGOMethod,false,false,menu);

        /*
        if(world_state.myGO !=null)
        {
            foreach(Component item in world_state.myGO.GetComponents(typeof(Component)).Where(c => c.hideFlags != HideFlags.HideInHierarchy))
            {
                menu = EditorHelper.GetPropertySelectionMenu(item.GetType(), typeof(object), selectedGOProperty, false, false, menu, "Bound(Self)/Property");
                menu = EditorHelper.GetFieldSelectionMenu(item.GetType(), typeof(object), selectedGOField, menu, "Bound(Self)/Field");
                menu = EditorHelper.GetMethodSelectionMenu(item.GetType(), typeof(object), selectedGOMethod, false,false, menu, "Bound(Self)/Method");
            }
        }
        foreach (var item in UserTypes.GetPreferedTypeList(typeof(object),true))
        {
            menu = EditorHelper.GetStaticPropertySelectionMenu(item.GetType(), typeof(object), selectedGOProperty, false, false, menu, "Bound(Self)/Property");
            menu = EditorHelper.GetStaticFieldSelectionMenu(item.GetType(), typeof(object), selectedGOField, menu, "Bound(Self)/Field");
            menu = EditorHelper.GetStaticMethodSelectionMenu(item.GetType(), typeof(object), selectedGOMethod, false, false, menu, "Bound(Self)/Method");

        }
        */
        menu.ShowAsContext();
        e.Use();
    }
    if (testFun.method != null)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        GUILayout.Label("Value");
        GUILayout.EndHorizontal();
    }
    else
    {
        EditorGUILayout.HelpBox("No variables added", MessageType.Info);
    }




}
}
#endif