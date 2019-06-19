using System;
using System.Collections.Generic;

namespace GOAP.Helper
{

    public static class OperatorHelper
    {
        static string[] op_array_name =
        {
            "OperatorSet",//0
            "OperatorEqual",//1
            "OperatorUnequal",//2
            "OperatorEqualHight",//3
            "OperatorEqualLess",//4
            "OperatorHight",//5
            "OperatorLess",//6
            "OperatorAddOne",//7
            "OperatorRemoveOne",//8
            "OperatorAdd",//9
            "OperatorRemove"//10
        };
        static string[] op_array_name_symbol =
       {
             "=",//0
            "==",//1
            "!=",//2
            ">=",//3
            "<=",//4
            ">",//5
            "<",//6
            "++",//7
            "--",//8
            "+=",//9
            "-="//10
        };
        private static Dictionary<int, List<string>> operator_data = new Dictionary<int, List<string>>()
        {
            {operator_name,new List<string>(op_array_name)},
            {operator_symbol,new List<string>(op_array_name_symbol)}
        };
        const int operator_name = 0;
        const int operator_symbol = 1;

        const int op_set = 0;
        const int op_equal = 1;
        const int op_unequal = 2;
        const int op_equal_or_hight = 3;
        const int op_equal_or_less =4;
        const int op_hight =5;
        const int op_less =6;
        const int op_add_one = 7;
        const int op_remove_one = 8;
        const int op_add = 9;
        const int op_remove = 10;        

        public static List<string> GetCheckOperatorForType(Type type)
        {
            List<string> ret = new List<string>();

            if(type == typeof(bool))
            {
                ret.Add(operator_data[operator_symbol][op_equal]);
                ret.Add(operator_data[operator_symbol][op_unequal]);
            }
            if (type == typeof(float) || type == typeof(int))
            {
                ret.Add(operator_data[operator_symbol][op_equal]);
                ret.Add(operator_data[operator_symbol][op_unequal]);
                ret.Add(operator_data[operator_symbol][op_equal_or_hight]);
                ret.Add(operator_data[operator_symbol][op_equal_or_less]);
                ret.Add(operator_data[operator_symbol][op_hight]);
                ret.Add(operator_data[operator_symbol][op_less]);
            }
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                ret.Add(operator_data[operator_symbol][op_equal]);
                ret.Add(operator_data[operator_symbol][op_unequal]);
            }
            return ret;
        }

        public static List<string> GetSetOperatorForType(Type type)
        {
            List<string> ret = new List<string>();

            if (type == typeof(bool))
            {
                ret.Add(operator_data[operator_symbol][op_set]);
            }
            if (type == typeof(float) || type == typeof(int))
            {
                ret.Add(operator_data[operator_symbol][op_set]);
                ret.Add(operator_data[operator_symbol][op_add_one]);
                ret.Add(operator_data[operator_symbol][op_remove_one]);
                ret.Add(operator_data[operator_symbol][op_add]);
                ret.Add(operator_data[operator_symbol][op_remove]);
            }
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                ret.Add(operator_data[operator_symbol][op_set]);
            }
            return ret;
        }
        public static string GetSymbolName(string symbol)
        {
            int index = operator_data[operator_symbol].IndexOf(symbol);
            if(index==-1)
                return "<b>NULL</b>";
            return operator_data[operator_name][index];
        }

    }
}