using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GOAP.Helper
{
    public static class ReleflectionHelper
    {
        private const BindingFlags flagAll = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        private static List<Assembly> _assemblies;
        private static List<Assembly> assemblies
        {
            get
            {
                if (_assemblies == null)
                    _assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

                return _assemblies;
            }
        }
        private static Dictionary<string, Type> type_map = new Dictionary<string, Type>();

        public static Type GetType(string typeName)
        {
            Type ret = null;

            if (type_map.TryGetValue(typeName, out ret))
                return ret;

            ret = Type.GetType(typeName);

            if (ret != null)
                return type_map[typeName] = ret;

            foreach (Assembly item in assemblies)
            {
                /*
                 *if (item == null)
                    continue;
                ret = item.GetType(type_name);
                */
                try
                {
                    ret = item.GetType(typeName);
                }
                catch
                {
                    continue;
                }
                if (ret != null)
                    return type_map[typeName] = ret;

            }
            foreach (Type item in GetAllType())
            {
                if (item.Name == typeName)
                    return type_map[typeName] = item;
            }

            UnityEngine.Debug.LogWarning("Can find Type by name");
            return null;
        }
        public static Type[] GetAllType()
        {
            var ret = new List<Type>();
            foreach (Assembly it in assemblies)
            {
                try
                {
                    ret.AddRange(it.RHelperGetExportedType());
                }
                catch
                {
                    continue;
                }
            }
            return ret.ToArray();
        }
        public static string FriendlyName(this Type type, bool trueSignature = false)
        {
            if (type == null)
                return null;

            if (!trueSignature && type == typeof(UnityEngine.Object))
            {
                return "UnityObject";
            }
            string str = trueSignature ? type.FullName : type.Name;

            if (!trueSignature)
            {
                str = str.Replace("Single", "Float");
                str = str.Replace("Int32", "Integer");
            }
            if (type.RHelperIsGenericParameter())
            {
                str = "T";
            }
            if (type.RHelperIsGenericType())
            {
                str = trueSignature ? type.FullName : type.Name;

                Type[] argument = type.RHelperGetGenericArguments();
                if (argument.Length != 0)
                {
                    str = str.Replace("`" + argument.Length.ToString(), "");

                    str += "<";
                    for (int i = 0; i < argument.Length; i++)
                    {
                        str += (i == 0 ? "" : ", ") + argument[i].FriendlyName(trueSignature);
                    }
                    str += ">";
                }
            }
            return str;
        }
        private static Dictionary<Type, FieldInfo[]> fieldsType = new Dictionary<Type, FieldInfo[]>();
        public static FieldInfo[] RHelperGetFields(this Type type)
        {
            FieldInfo[] fields;
            if (!fieldsType.TryGetValue(type, out fields))
            {
                fields = type.GetFields(flagAll);

                fieldsType[type] = fields;
            }
            return fields;
        }
        private static Type[] RHelperGetExportedType(this Assembly value)
        {
            return value.GetExportedTypes();
        }
        private static Type[] RHelperGetGenericArguments(this Type value)
        {
            return value.GetGenericArguments();
        }
        public static Type RHelperReflectedType(this Type type)
        {
            return type.ReflectedType;
        }
        public static Type RHelperReflectedType(this MemberInfo member)
        {
            return member.ReflectedType;
        }
        public static bool RHelperIsAssignableFrom(this Type main, Type second)
        {
            return main.IsAssignableFrom(second);
        }
        public static bool RHelperIsSubclassFrom(this Type main, Type second)
        {
            return main.IsSubclassOf(second);
        }
        public static bool RHelperIsAbstract(this Type type)
        {
            return type.IsAbstract;
        }
        public static bool RHelperIsValueType(this Type type)
        {
            return type.IsValueType;
        }
        public static bool RHelperIsArray(this Type type)
        {
            return type.IsArray;
        }
        public static bool RHelperIsInterface(this Type type)
        {
            return type.IsInterface;
        }
        public static bool RHelperIsGenericParameter(this Type type)
        {
            return type.IsGenericParameter;
        }
        public static bool RHelperIsGenericType(this Type type)
        {
            return type.IsGenericType;
        }
        public static MethodInfo RHelperGetGetMethodInfo(this PropertyInfo property)
        {
            return property.GetGetMethod();
        }
        public static MethodInfo RHelperGetSetMethodInfo(this PropertyInfo property)
        {
            return property.GetSetMethod();
        }
        public static PropertyInfo RHelperGetPropertyInfo(this Type type, string name)
        {
            return type.GetProperty(name, flagAll);
        }
        public static bool RHelperIsFieldReadOnly(this FieldInfo field)
        {
            return field.IsInitOnly || field.IsLiteral;
        }
        public static FieldInfo RHelperGetFieldInfo(this Type type, string name)
        {
            return type.GetField(name, flagAll);
        }

        public static Type RHelperGenericType(this Type type, Type[] arrayType)
        {
            return type.MakeGenericType(arrayType);
        }

        public static T RHelperCreateDelegate<T>(this MethodInfo method, object instance)
        {
            return (T)(object)method.RHelperCreateDelegate(typeof(T), instance);
        }
        public static T RHelperGetAttribute<T>(this Type type, bool inherited)
        {
            return (T)type.GetCustomAttributes(typeof(T), inherited).FirstOrDefault();
        }
        public static Delegate RHelperCreateDelegate(this MethodInfo method, Type type, object instance)
        {
            return Delegate.CreateDelegate(type, instance, method);
        }
        public static PropertyInfo RHelperGetBaseDefinition(this PropertyInfo property)
        {
            MethodInfo method = property.GetAccessors(true)[0];
            if (method == null)
                return null;

            MethodInfo baseDefinition = method.GetBaseDefinition();
            if (baseDefinition == method)
            {
                return property;
            }
            var parameter = property.GetIndexParameters().Select(p => p.ParameterType).ToArray();
            return baseDefinition.DeclaringType.GetProperty(property.Name, flagAll, null, property.PropertyType, parameter, null);
        }
        public static FieldInfo RHelperGetBaseDefinition(this FieldInfo field)
        {
            return field.DeclaringType.RHelperGetFieldInfo(field.Name);
        }
    }
}