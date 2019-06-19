using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEditor;
namespace GOAP.Helper
{
    public static partial class EditorHelper
    {

        private static readonly Dictionary<IList, object> picketObj = new Dictionary<IList, object>();
        private static readonly int mark_spacing = 2;
        public static void ReorderableList(IList list, System.Action<int> callback, UnityEngine.Object engineObject = null)
        {
            if (list.Count == 1)
            {
                callback(0);
                return;
            }
            if (!picketObj.ContainsKey(list))
            {
                picketObj[list] = null;
            }
            Event e = Event.current;
            Rect last_rect = new Rect();
            object picked_obj = picketObj[list];
            GUILayout.BeginVertical();
            for (int i = 0; i < list.Count; i++)
            {
                GUILayout.BeginVertical();
                callback(i);
                GUILayout.EndVertical();

                last_rect = GUILayoutUtility.GetLastRect();
                EditorGUIUtility.AddCursorRect(last_rect, MouseCursor.MoveArrow);

                if (picked_obj != null && picked_obj == list[i])
                    GUI.Box(last_rect, "");

                if (picked_obj != null && last_rect.Contains(e.mousePosition) && picked_obj != list[i])
                {
                    Rect mark_rect = new Rect(last_rect.x, last_rect.y - mark_spacing, last_rect.width, mark_spacing);

                    if (list.IndexOf(picked_obj) < i)
                        mark_rect.y = last_rect.yMax - mark_spacing;

                    GUI.Box(mark_rect, "");
                    if (e.type == EventType.MouseUp)
                    {
                        if (engineObject != null)
                            Undo.RecordObject(engineObject, "Reorder");

                        list.Remove(picked_obj);
                        list.Insert(i, picked_obj);
                        picketObj[list] = null;

                        if (engineObject != null)
                            EditorUtility.SetDirty(engineObject);
                    }
                }
                if (last_rect.Contains(e.mousePosition) && e.type == EventType.MouseDown)
                    picketObj[list] = list[i];
            }

            GUILayout.EndVertical();
            if (e.type == EventType.MouseUp)
                picketObj[list] = null;
        }
        public static GenericMenu GetTypeSelectionMenu(Type type, Action<Type> callback, GenericMenu menu = null, string subCategory = null)
        {
            if (menu == null)
                menu = new GenericMenu();
            if (subCategory != null)
                subCategory = subCategory + "/";
            GenericMenu.MenuFunction2 selected = delegate (object t)
            {
                callback((Type)t);
            };

            List<ScriptInfo> scriptInfos = GetScriptInfoFromType(type);

            foreach (ScriptInfo info in scriptInfos.Where(info => string.IsNullOrEmpty(info.category)))
            {
                menu.AddItem(new GUIContent(subCategory + info.name, info.description), false, info.type != null ? selected : null, info.type);
            }

            foreach (ScriptInfo info in scriptInfos.Where(info => !string.IsNullOrEmpty(info.category)))
            {
                menu.AddItem(new GUIContent(subCategory + info.category + "/" + info.name, info.description), false, info.type != null ? selected : null, info.type);
            }
            return menu;
        }
        public static GenericMenu GetPreferedTypesSelectionMenu(Type type, Action<Type> callback, bool showInterface = true, GenericMenu menu = null, string subCategory = null)
        {
            if (menu == null)
                menu = new GenericMenu();
            if (subCategory != null)
                subCategory = subCategory + "/";
            GenericMenu.MenuFunction2 selected = delegate (object t)
            {
                callback((Type)t);
            };
            Dictionary<Type, string> typeList = new Dictionary<Type, string>();

            foreach (Type item in UserTypes.GetPreferedTypeList(typeof(object), false))
            {
                if (type.IsAssignableFrom(item) || item.IsInterface && showInterface)
                {
                    string str = string.IsNullOrEmpty(item.Namespace) ? "No Namespace/" : (item.Namespace.Replace(".", "/") + "/");
                    string final_str = str + item.FriendlyName();
                    menu.AddItem(new GUIContent(subCategory + final_str), false, selected, item);
                    typeList.Add(typeof(List<>).MakeGenericType(new Type[] { item }), str);
                }
            }
            /*
            foreach (var item in typeList)
            {
                menu.AddItem(new GUIContent(subCategory + "List<T>/" + item.Value), false, selected, item.Key);
            }
            */

            // menu.AddItem(new GUIContent(sub_category + "Add Type"), false, selected,()=> { });

            return menu;
        }
        public static GenericMenu GetMethodSelectionMenu(Action<MethodInfo> callback, bool mustRead = true, bool mustWrite = true, GenericMenu menu = null)
        {

            return GetMethodSelectionMenu(BindingFlags.Public | BindingFlags.Instance, typeof(WSVariableParameter), typeof(object), callback, mustRead, mustWrite, menu);
        }

        public static GenericMenu GetMethodSelectionMenu(Type type, Type method, Action<MethodInfo> callback, bool mustRead = true, bool mustWrite = true, GenericMenu menu = null, string subMenu = null)
        {
            return GetMethodSelectionMenu(BindingFlags.Public | BindingFlags.Instance, type, method, callback, mustRead, mustWrite, menu, subMenu);
        }
        public static GenericMenu GetStaticMethodSelectionMenu(Type type, Type method, Action<MethodInfo> callback, bool mustRead = true, bool mustWrite = true, GenericMenu menu = null, string subMenu = null)
        {
            return GetMethodSelectionMenu(BindingFlags.Public | BindingFlags.Static, type, method, callback, mustRead, mustWrite, menu, subMenu);
        }
        static GenericMenu GetMethodSelectionMenu(BindingFlags flag, Type type, Type method, Action<MethodInfo> callback, bool mustRead = true, bool mustWrite = true, GenericMenu menu = null, string subMenu = null)
        {
            if (menu == null)
                menu = new GenericMenu();

            if (subMenu != null)
                subMenu += "/";

            GenericMenu.MenuFunction2 selected = delegate (object selected_property)
            {
                callback((MethodInfo)selected_property);
            };

            bool itemAdded = false;
            bool more = false;
            foreach (MethodInfo item in type.GetMethods(flag))
            {
            
               
                    if (item.DeclaringType != type)
                    {
                        more = true;
                    }
                    string category = more ? subMenu + type.FriendlyName() + "/More" : subMenu + type.FriendlyName();


                    menu.AddItem(new GUIContent(string.Format("{0}/{1} : {2}", category, item.Name, item.DeclaringType.FriendlyName())), false, selected, item);
                    itemAdded = true;
                

            }
            if (!itemAdded)
                menu.AddDisabledItem(new GUIContent(subMenu + type.FriendlyName()));

            return menu;

        }

        public static GenericMenu GetPropertySelectionMenu(Type type, Type property, Action<PropertyInfo> callback, bool mustRead = true, bool mustWrite = true, GenericMenu menu = null, string subMenu = null)
        {
            return GetPropertySelectionMenu(BindingFlags.Public | BindingFlags.Instance, type, property, callback, mustRead, mustWrite, menu, subMenu);
        }
        public static GenericMenu GetStaticPropertySelectionMenu(Type type, Type property, Action<PropertyInfo> callback, bool mustRead = true, bool mustWrite = true, GenericMenu menu = null, string subMenu = null)
        {
            return GetPropertySelectionMenu(BindingFlags.Public | BindingFlags.Static, type, property, callback, mustRead, mustWrite, menu, subMenu);
        }
        static GenericMenu GetPropertySelectionMenu(BindingFlags flag, Type type, Type property, Action<PropertyInfo> callback, bool mustRead = true, bool mustWrite = true, GenericMenu menu = null, string subMenu = null)
        {
            if (menu == null)
                menu = new GenericMenu();

            if (subMenu != null)
                subMenu += "/";

            GenericMenu.MenuFunction2 selected = delegate (object selected_property)
            {
                callback((PropertyInfo)selected_property);
            };

            bool itemAdded = false;
            bool more = false;
            foreach (PropertyInfo item in type.GetProperties(flag))
            {
                if (!item.CanRead && mustRead || !item.CanWrite && mustRead)
                    continue;

                if (property.IsAssignableFrom(item.PropertyType))
                {
                    if (item.DeclaringType != type)
                    {
                        more = true;
                    }
                    string category = more ? subMenu + type.FriendlyName() + "/More" : subMenu + type.FriendlyName();

                    PropertyInfo propertyInfo = item.RHelperGetBaseDefinition();

                    menu.AddItem(new GUIContent(string.Format("{0}/{1} : {2}", category, propertyInfo.Name, propertyInfo.PropertyType.FriendlyName())), false, selected, propertyInfo);
                    itemAdded = true;
                }

            }
            if (!itemAdded)
                menu.AddDisabledItem(new GUIContent(subMenu + type.FriendlyName()));

            return menu;

        }

        public static GenericMenu GetFieldSelectionMenu(Type type, Type fieldType, Action<FieldInfo> callback, GenericMenu menu = null, string subMenu = null)
        {
            return GetFieldSelectionMenu(BindingFlags.Public | BindingFlags.Instance, type, fieldType, callback, menu, subMenu);
        }
        public static GenericMenu GetStaticFieldSelectionMenu(Type type, Type fieldType, Action<FieldInfo> callback, GenericMenu menu = null, string subMenu = null)
        {
            return GetFieldSelectionMenu(BindingFlags.Public | BindingFlags.Static, type, fieldType, callback, menu, subMenu);
        }
        static GenericMenu GetFieldSelectionMenu(BindingFlags flag, Type type, Type fieldType, Action<FieldInfo> callback, GenericMenu menu = null, string subMenu = null)
        {
            if (menu == null)
                menu = new GenericMenu();

            if (subMenu != null)
                subMenu += "/";

            GenericMenu.MenuFunction2 selected = delegate (object selected_property)
            {
                callback((FieldInfo)selected_property);
            };

            bool itemAdded = false;
            bool more = false;
            foreach (FieldInfo item in type.GetFields(flag).Where(item => fieldType.IsAssignableFrom(item.FieldType)))
            {

                if (item.DeclaringType != type)
                    more = true;

                string category = more ? subMenu + type.FriendlyName() + "/More" : subMenu + type.FriendlyName();

                FieldInfo fieldInfo = item.RHelperGetBaseDefinition();

                menu.AddItem(new GUIContent(string.Format("{0}/{1} : {2}", category, fieldInfo.Name, fieldInfo.FieldType.FriendlyName())), false, selected, fieldInfo);
                itemAdded = true;


            }
            if (!itemAdded)
                menu.AddDisabledItem(new GUIContent(subMenu + type.FriendlyName()));

            return menu;

        }
    }
}