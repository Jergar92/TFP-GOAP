using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace GOAP.Framework
{
    public interface IWorldState
    {
        event Action<WSVariable> onVariableAdded;
        event Action<WSVariable> onVariableRemoved;

        event Action<WSFunctionVariable> onFunctionVariableAdded;
        event Action<WSFunctionVariable> onFunctionVariableRemoved;

        string name { get; set; }

        Dictionary<string, WSVariable> myVariables { get; set; }

        Dictionary<string, WSFunctionVariable> myFunctionVariables { get; set; }

        GameObject myGO { get; }

        WSVariable AddVariable(WSVariable variable);
        WSVariable AddVariable(string name, Type type);
        WSFunctionVariable AddFunctionVariable(WSFunctionVariable method, bool value);
        WSFunctionVariable AddFunctionVariable(MethodInfo variable, GameObject agent = null);

        WSVariable RemoveVariable(string name);
        WSFunctionVariable RemoveFunctionVariable(string name);
        WSVariable GetVariable(string name, Type type = null);
        WSVariable GetVariableByID(string ID);
        WSFunctionVariable GetFunctionVariable(string name, Type type = null);
        WSFunctionVariable GetFunctionVariableByID(string ID);
        WSVariable<T> GetVariable<T>(string name);
        T GetTValue<T>(string name);
        bool Exist(WSVariable variable);
        bool Exist(string name);
        void Clear();
        WSVariable SetValue(string name, object value);
        string[] GetVariableNames();
        string[] GetFunctionVariableNames();

        string[] GetVariableNames(Type type);
    }
}