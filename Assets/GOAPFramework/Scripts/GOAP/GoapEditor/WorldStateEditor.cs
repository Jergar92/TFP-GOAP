#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using GOAP.Helper;
using UnityEditor;
namespace GOAP.Framework
{
    public static class WorldStateEditor
    {
        class ReorderingState
        {
            public List<WSVariable> list;
            public bool is_reordering = false;
            public ReorderingState(List<WSVariable> list)
            {
                this.list = list;
            }
        }

        class ReorderingFunctionState
        {
            public List<WSFunctionVariable> list;
            public bool is_reordering = false;
            public ReorderingFunctionState(List<WSFunctionVariable> list)
            {
                this.list = list;
            }
        }
        private static readonly Dictionary<IWorldState, ReorderingState> temp_states = new Dictionary<IWorldState, ReorderingState>();
        private static readonly Dictionary<IWorldState, ReorderingFunctionState> tempFuntionStates = new Dictionary<IWorldState, ReorderingFunctionState>();

        private static readonly int variable_property_button_width = 16;
        private static readonly int variable_property_button_height = 16;

        private static readonly int variable_delete_button_width = 16;
        private static readonly int variable_delete_button_height = 16;

        public static WSVariable pickedVariable { get; set; }
        public static IWorldState pickedVariableWorldState { get; set; }
        public static void ShowVariables(IWorldState _worldState, UnityEngine.Object parent)
        {
            GUI.skin.label.richText = true;
            Event e = Event.current;
            var layout_options = new GUILayoutOption[] { GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true), GUILayout.Height(20) };


            UndoHelper.CheckUndo(parent, "WorldState Inspector");
            if (GUILayout.Button("Add Variable"))
            {
                System.Action<System.Type> selectedVariable = (t) =>
                {
                    string str_name = "my" + t.FriendlyName();
                    while (_worldState.GetVariable(str_name) != null && _worldState.GetFunctionVariable(str_name) != null)
                    {
                        str_name += "-";
                    }                

                    _worldState.AddVariable(str_name, t);
                };

                System.Action<MethodInfo> selectedGOMethod = (f) =>
                {
                    string str_name = f.Name;
                    while (_worldState.GetVariable(str_name) != null && _worldState.GetFunctionVariable(str_name) != null)
                    {
                        str_name += "-";
                    }
                    WSFunctionVariable variable = _worldState.AddFunctionVariable(f);
                };

                GenericMenu menu = new GenericMenu();

                menu = EditorHelper.GetPreferedTypesSelectionMenu(typeof(object), selectedVariable, true, menu, "New");

                menu = EditorHelper.GetMethodSelectionMenu(selectedGOMethod,false,false,menu);

      
                menu.AddItem(new GUIContent("Add Separator"), false, () => { selectedVariable(typeof(WSSeparator)); });
                menu.ShowAsContext();
                e.Use();
            }
            if (_worldState.myVariables.Count != 0)
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
            if (!temp_states.ContainsKey(_worldState))
            {
                temp_states.Add(_worldState, new ReorderingState(_worldState.myVariables.Values.ToList()));
            }
            if (!temp_states[_worldState].is_reordering)
            {
                temp_states[_worldState].list = _worldState.myVariables.Values.ToList();
            }
            // List<string> variable_names = 
            EditorHelper.ReorderableList(temp_states[_worldState].list, delegate (int i)
             {

                 WSVariable data = temp_states[_worldState].list[i];
                 if (data == null)
                 {
                     GUILayout.Label("Null!");
                     return;
                 }
                 GUILayout.Space(data.myType == typeof(WSSeparator) ? 5 : 0);

                 GUILayout.BeginHorizontal();

                 if (!Application.isPlaying)
                 {
                     GUILayout.Box("", GUILayout.Width(6));

                     if (e.type == EventType.MouseDown && e.button == 0 && GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
                     {
                         temp_states[_worldState].is_reordering = true;
                         if (data.myType != typeof(WSSeparator))
                         {
                             pickedVariable = data;
                             pickedVariableWorldState = _worldState;
                         }
                     }

                     if (temp_states[_worldState].list.Where(v => v != data).Select(v => v.name).Contains(data.name))
                     {
                         GUI.backgroundColor = Color.red;

                     }
                     GUI.enabled = !data.isProtected;

                     if (data.myType != typeof(WSSeparator))
                     {
                         data.name = EditorGUILayout.TextField(data.name);
                         EditorGUI.indentLevel = 0;
                     }
                     else
                     {
                         WSSeparator separator = (WSSeparator)data.value;

                         if (separator.isEditingName)
                             data.name = EditorGUILayout.TextField(data.name);
                         else
                             GUILayout.Label(string.Format("<b>{0}</b>", data.name).ToUpper());

                         if (!separator.isEditingName)
                         {
                             if (e.type == EventType.MouseDown && e.button == 0 && e.clickCount == 2 && GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
                             {
                                 separator.isEditingName = true;
                                 GUIUtility.keyboardControl = 0;
                             }
                         }
                         if (separator.isEditingName)
                         {
                             if ((e.isKey && e.keyCode == KeyCode.Return) && (e.rawType == EventType.MouseUp && !GUILayoutUtility.GetLastRect().Contains(e.mousePosition)))
                             {
                                 separator.isEditingName = false;
                                 GUIUtility.keyboardControl = 0;
                             }
                         }
                         data.value = separator;
                     }
                     GUI.enabled = true;
                     GUI.backgroundColor = Color.white;

                 }
                 else
                 {
                     if (data.myType != typeof(WSSeparator))
                     {
                         GUILayout.Label(data.name);

                     }
                     else
                     {
                         GUILayout.Label(string.Format("<b>{0}</b>", data.name).ToUpper());
                     }
                 }

                 if (data.myType != typeof(WSSeparator))
                 {
                     ShowVariableGUI(data, _worldState, parent, layout_options);
                 }
                 else
                 {
                     GUILayout.Space(0);

                 }
                 if (!Application.isPlaying && GUILayout.Button("X", GUILayout.Width(variable_delete_button_width), GUILayout.Height(variable_delete_button_height)))
                 {
                     if (EditorUtility.DisplayDialog("Delete Variable '" + data.name + "'", "Delete '" + data.name + "'?", "Yes", "No"))
                     {
                         temp_states[_worldState].list.Remove(data);
                         _worldState.RemoveVariable(data.name);
                         GUIUtility.keyboardControl = 0;
                         GUIUtility.hotControl = 0;
                     }
                 }
                 GUILayout.EndHorizontal();
             }, parent);

            if ((GUI.changed && !temp_states[_worldState].is_reordering) || e.rawType == EventType.MouseUp)
            {
                temp_states[_worldState].is_reordering = false;
                EditorApplication.delayCall += () =>
                {
                    pickedVariable = null;
                    pickedVariableWorldState = null;
                };
                try
                {
                    _worldState.myVariables = temp_states[_worldState].list.ToDictionary(d => d.name, d => d);
                }
                catch
                {
                    Debug.LogError("World State have duplicated names");
                }
            }

            //-----------------------------------------
            EditorHelper.Separator();
            if (_worldState.myFunctionVariables.Count != 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Function");
                GUILayout.Label("Value");
                GUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("No function variables added", MessageType.Info);
            }

            if (!tempFuntionStates.ContainsKey(_worldState))
            {
                tempFuntionStates.Add(_worldState, new ReorderingFunctionState(_worldState.myFunctionVariables.Values.ToList()));
            }
            if (!tempFuntionStates[_worldState].is_reordering)
            {
                tempFuntionStates[_worldState].list = _worldState.myFunctionVariables.Values.ToList();
            }
            // List<string> variable_names = 
            EditorHelper.ReorderableList(tempFuntionStates[_worldState].list, delegate (int i)
            {

                WSFunctionVariable data = tempFuntionStates[_worldState].list[i];
                if (data == null)
                {
                    GUILayout.Label("Null!");
                    return;
                }
                GUILayout.Space(0);

                GUILayout.BeginHorizontal();

                if (!Application.isPlaying)
                {
                    GUILayout.Box("", GUILayout.Width(6));

                    if (e.type == EventType.MouseDown && e.button == 0 && GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
                    {
                        tempFuntionStates[_worldState].is_reordering = true;
              
                    }

                    if (tempFuntionStates[_worldState].list.Where(v => v != data).Select(v => v.name).Contains(data.name))
                    {
                        GUI.backgroundColor = Color.red;

                    }
                   
                    data.name = EditorGUILayout.TextField(data.name);
              
                    GUI.enabled = true;
                    GUI.backgroundColor = Color.white;

                }
                else
                { 
                    GUILayout.Label(data.name);

                }
                //ShowVariableGUI(data, _worldState, parent, layout_options);               
                if (!Application.isPlaying && GUILayout.Button("X", GUILayout.Width(variable_delete_button_width), GUILayout.Height(variable_delete_button_height)))
                {
                    if (EditorUtility.DisplayDialog("Delete Variable '" + data.name + "'", "Delete '" + data.name + "'?", "Yes", "No"))
                    {
                        tempFuntionStates[_worldState].list.Remove(data);
                        _worldState.RemoveFunctionVariable(data.name);
                        GUIUtility.keyboardControl = 0;
                        GUIUtility.hotControl = 0;
                    }
                }
                GUILayout.EndHorizontal();
            }, parent);

            if ((GUI.changed && !tempFuntionStates[_worldState].is_reordering) || e.rawType == EventType.MouseUp)
            {
                tempFuntionStates[_worldState].is_reordering = false;
                EditorApplication.delayCall += () =>
                {
                    pickedVariable = null;
                    pickedVariableWorldState = null;
                };
                try
                {
                   
                    _worldState.myFunctionVariables = tempFuntionStates[_worldState].list.ToDictionary(d => d.name, d => d);
                }
                catch
                {
                    Debug.LogError("World State have duplicated names");
                }
            }



            //------------------------------





            //polish
            UndoHelper.CheckDirty(parent);
  
        }

        static void ShowVariableGUI(WSVariable data, IWorldState _worldState, UnityEngine.Object parent, GUILayoutOption[] layout_options = null)
        {
            if (data.isBinded)
            {
                /*
                int index = data.propertyPath.LastIndexOf('.');
                string type_str = data.propertyPath.Substring(0, index);
                string member_str = data.propertyPath.Substring(index, index + 1);
                GUILayout.Label(string.Format(".{0} ({1})", member_str, type_str.Split('.').Last()));
                */
            }
            else
            {
                GUI.enabled = !data.isProtected;
                data.value = VariableField(data, parent, layout_options);
                GUI.enabled = true;

            }
            if (!Application.isPlaying && GUILayout.Button("!", GUILayout.Width(variable_property_button_width), GUILayout.Height(variable_property_button_height)))
            {
                /*
                System.Action<PropertyInfo> select_go_prop = (p) =>
                {
                    data.BindProperty(p);
                };

                System.Action<FieldInfo> select_go_field = (f) =>
                {
                    data.BindProperty(f);
                };
                */
                GenericMenu menu = new GenericMenu();
                /*
                if (_worldState.myGO != null)
                {
                    foreach (Component item in _worldState.myGO.GetComponents(typeof(Component)).Where(c => c.hideFlags != HideFlags.HideInHierarchy))
                    {
                        menu = EditorHelper.GetPropertySelectionMenu(item.GetType(), data.myType, select_go_prop, false, false, menu, "Bound(Self)/Property");
                        menu = EditorHelper.GetFieldSelectionMenu(item.GetType(), data.myType, select_go_field, menu, "Bound(Self)/Field");

                    }
                }
                foreach (var item in UserTypes.GetPreferedTypeList(typeof(object), true))
                {
                    menu = EditorHelper.GetStaticPropertySelectionMenu(item.GetType(), data.myType, select_go_prop, false, false, menu, "Bound(Self)/Property");
                    menu = EditorHelper.GetStaticFieldSelectionMenu(item.GetType(), data.myType, select_go_field, menu, "Bound(Self)/Field");

                }
                */
                menu.AddItem(new GUIContent("Protected"), data.isProtected, () => { data.isProtected = !data.isProtected; });

                if (_worldState.myGO != null)
                {
                    /*
                        menu.AddSeparator("/");
                        if (data.isBinded)                
                            menu.AddItem(new GUIContent("UnBind"), false, () => { data.UnBindProperty(); });                
                        else
                            menu.AddDisabledItem(new GUIContent("UnBind"));
                    */
                }
                menu.ShowAsContext();
                Event.current.Use();
            }

        }

        static object VariableField(WSVariable data, UnityEngine.Object context, GUILayoutOption[] layoutOptions)
        {
            object dataObj = data.value;
            Type type = data.myType;


            var isUnityObj = (typeof(Component).IsAssignableFrom(type) ||
                             type == typeof(GameObject) ||
                             type.IsInterface);
            if (typeof(UnityEngine.Object).IsAssignableFrom(type) || type.IsInterface)
            {
                return EditorGUILayout.ObjectField((UnityEngine.Object)dataObj, type, isUnityObj, layoutOptions);
            }

            if (type == typeof(bool))
            {
                return EditorGUILayout.Toggle((bool)dataObj, layoutOptions);
            }
            if (type == typeof(int))
            {
                return EditorGUILayout.IntField((int)dataObj, layoutOptions);
            }
            if (type == typeof(float))
            {
                return EditorGUILayout.FloatField((float)dataObj, layoutOptions);
            }
            if (type == typeof(string))
            {
                return EditorGUILayout.TextField((string)dataObj, layoutOptions);
            }
            if (type == typeof(Vector2))
            {
                return EditorGUILayout.Vector2Field("", (Vector2)dataObj, layoutOptions);
            }
            if (type == typeof(Vector3))
            {
                return EditorGUILayout.Vector3Field("", (Vector3)dataObj, layoutOptions);
            }
            if (type == typeof(Vector4))
            {
                return EditorGUILayout.Vector4Field("", (Vector4)dataObj, layoutOptions);
            }

            return dataObj;

        }
        static List<string> GetVariableNames()
        {
            List<string> ret = new List<string>();


            return ret;
        }

    }
}
#endif