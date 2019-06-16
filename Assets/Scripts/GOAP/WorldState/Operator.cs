using System;
using System.Collections.Generic;
using System.Reflection;
using GOAP.Helper;

namespace GOAP.Framework.Internal
{
    [Serializable]
    public class Operator
    {
        private string current_operator="<b>NULL</b>";
        private string operator_symbol= "<b>NULL</b>";

        public string currentOperator
        {
            get
            {
                return current_operator;
            }
            set
            {
                if(current_operator!=value)
                {
                    current_operator = value;
                }
            }
        }
        public string symbol
        {
            get
            {
                if (string.IsNullOrEmpty(operator_symbol))
                    return "<b>NULL</b>";
                return operator_symbol;
            }
            set
            {
                if (operator_symbol != value)
                {
                    operator_symbol = value;
                    current_operator = OperatorHelper.GetSymbolName(current_operator);

                }
            }
        }
        public List<string> GetOperatorForType(Type type)
        {
            return null;
        }

        public object Use(object[] parameters)
        {

            object ret;
            string method_name = "Use" + current_operator;
            MethodInfo info = GetType().GetMethod(method_name);
            ret = info.Invoke(this, parameters);
            return ret;
        }

        private object GetSimbol(string operator_name)
        {

            object ret;
            string method_name = "GetSimbol" + operator_name;
            MethodInfo info = GetType().GetMethod(method_name);
            ret = info.Invoke(this,null);
            return ret;
        }
        private bool UseOperatorEqual(object item_1, object item_2)
        {
            bool ret = true;
            if(item_1.GetType()==item_2.GetType())
            {
                FieldInfo[] fields = item_1.GetType().GetFields();
                foreach(FieldInfo field in fields)
                {
                    if (field.GetValue(item_1) != field.GetValue(item_2))
                    {
                        ret = false;
                        break;
                    }
                }
            }
            else
            {
                ret = false;
            }
            return ret;
        }
        private bool UseOperatorUnequal(object item_1, object item_2)
        {
            return !UseOperatorEqual(item_1, item_2);
        }
        private bool UseOperatorEqualHight(object item_1, object item_2)
        {
            bool ret = true;

            return ret;
        }
        private bool UseOperatorEqualLess(object item_1, object item_2)
        {
            bool ret = true;

            return ret;
        }
        private bool UseOperatorHight(object item_1, object item_2)
        {
            bool ret = true;

            return ret;
        }
        private bool UseOperatorLess(object item_1, object item_2)
        {
            bool ret = true;

            return ret;
        }
    }
}