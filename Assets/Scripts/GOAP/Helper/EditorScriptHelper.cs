using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEditor;
namespace GOAP.Helper
{
    partial class EditorHelper
    {

        public class ScriptInfo
        {
            public Type type;
            public string name;
            public string category;
            public string description;

            public ScriptInfo(Type type, string name, string category)
            {
                this.type = type;
                this.name = name;
                this.category = category;
                if (type != null)
                {
                    //TODO get description attribute
                }
            }
        }
        private static Dictionary<Type, List<ScriptInfo>> script_infos = new Dictionary<Type, List<ScriptInfo>>();
        public static List<ScriptInfo> GetScriptInfoFromType(Type type, Type extraType = null)
        {
            List<ScriptInfo> ret_info;

            if (script_infos.TryGetValue(type, out ret_info))
                return ret_info;

            ret_info = new List<ScriptInfo>();

            List<Type> sub_types = GetSubTypes(type);

            if (type.IsGenericTypeDefinition)
            {
                sub_types = new List<Type> { type };
            }
            foreach (Type sub_type in sub_types)
            {
                if (sub_type.IsAbstract)
                    continue;

                string scriptName;
                string scriptCategory;

                NameAttribute attributeName = sub_type.GetCustomAttributes(typeof(NameAttribute), false).FirstOrDefault() as NameAttribute;
                scriptName = attributeName != null ? attributeName.name : sub_type.FriendlyName();

                CategoryAttribute category_attribute = sub_type.GetCustomAttributes(typeof(CategoryAttribute), false).FirstOrDefault() as CategoryAttribute;
                scriptCategory = category_attribute != null ? category_attribute.category : string.Empty;

                if (sub_type.IsGenericTypeDefinition && sub_type.GetGenericArguments().Length == 1)
                {
                    var argument = sub_type.GetGenericArguments()[0];
                    var constrains = argument.GetGenericParameterConstraints();
                    var constrainType = constrains.Length == 0 ? typeof(object) : constrains[0];
                    var types = UserTypes.GetPreferedTypeList(constrainType, false);

                    if (extraType != null)
                        types.Add(extraType);

                    foreach (Type item in types)
                    {
                        Type generic_type = sub_type.MakeGenericType(new Type[] { item });
                        string finalCategoryPath = (string.IsNullOrEmpty(scriptCategory)) ? "" : (scriptCategory + "/");

                        finalCategoryPath = finalCategoryPath.Replace("<T>", " (T)");
                        finalCategoryPath += "/" + (string.IsNullOrEmpty(type.Namespace));
                        string finalName = scriptName.Replace("<T>", string.Format(" ({0})", type.FriendlyName()));

                        ret_info.Add(new ScriptInfo(generic_type, finalName, finalCategoryPath));
                    }
                }
                else
                {
                    ret_info.Add(new ScriptInfo(sub_type, scriptName, scriptCategory));
                }

            }
            return script_infos[type] = ret_info;

        }

        private static Dictionary<Type, List<Type>> subTypes = new Dictionary<Type, List<Type>>();
        public static List<Type> GetSubTypes(Type type)
        {
            List<Type> ret;

            if (subTypes.TryGetValue(type, out ret))
                return ret;

            ret = new List<Type>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type exportedType in assembly.GetExportedTypes().Where(type_value => type_value.IsSubclassOf(type)))
                    {
                        ret.Add(exportedType);
                    }
                }
                catch
                {
                    Debug.Log(assembly.FullName + " not added on SubTypes");
                    continue;
                }
            }

            return subTypes[type] = ret;
        }
    }
}