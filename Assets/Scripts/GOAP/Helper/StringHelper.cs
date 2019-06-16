using System;
using System.Collections;
using System.Linq;

namespace GOAP.Helper
{
    public static class StringHelper
    {
        public static string ToStringAdvanced(this object obj)
        {
            if (obj == null || obj.Equals(null))
                return "NULL";
            if (obj is string)
                return string.Format("\"{0}\"", (string)obj);
            if (obj is UnityEngine.Object)
                return (obj as UnityEngine.Object).name;

            return obj.ToString();
        }
    }
}