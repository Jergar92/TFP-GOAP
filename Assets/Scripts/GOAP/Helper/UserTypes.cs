using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEditor;
namespace GOAP.Helper
{
    public static class UserTypes
    {


        private static List<Type> preferedList;
        public static string defaultPreferedList
        {
            get
            {
                List<Type> typeList = new List<Type>
            {
                typeof(object),

                typeof(string),
                typeof(int),
                typeof(float),
                typeof(bool),

                typeof(Vector2),
                typeof(Vector3),
                typeof(Vector4),

                typeof(GameObject),
                typeof(Transform),

            };
                return string.Join("|", typeList.OrderBy(t => t.Name).OrderBy(t => t.Namespace).Select(t => t.FullName).ToArray());
            }
        }
        public static List<Type> GetPreferedTypeList(Type type, bool dontFilter)
        {
            if (preferedList == null)
            {
                preferedList = new List<Type>();

                foreach (var item in EditorPrefs.GetString("", defaultPreferedList).Split('|'))
                {
                    Type value = ReleflectionHelper.GetType(item);
                    if (value == null)
                    {
                        Debug.LogWarning(string.Format("Type with name '{0}' dont found", item));
                        continue;
                    }
                    preferedList.Add(value);

                }
            }
            List<Type> ret = preferedList.Where(t => type.IsAssignableFrom(t) && !t.IsGenericType).OrderBy(t => t.Name).OrderBy(t => t.Namespace).ToList();

            if (!dontFilter)
            {
                foreach (Type item in ret.ToArray())
                {
                    if (!item.IsEnum &&
                        item.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Length == 0 &&
                        item.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Length == 0)
                    {
                        ret.Remove(item);
                    }
                }
            }
            return ret;
        }
    }
}