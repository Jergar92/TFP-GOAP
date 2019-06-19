using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GOAP.Helper;
namespace GOAP.Framework.Internal
{
    [Serializable]
    public class WorldStateData : IWorldState
    {
        public event Action<WSVariable> onVariableAdded;
        public event Action<WSVariable> onVariableRemoved;

        public event Action<WSFunctionVariable> onFunctionVariableAdded;
        public event Action<WSFunctionVariable> onFunctionVariableRemoved;
        [SerializeField]
        private string _name;
        [SerializeField]
        private Dictionary<string, WSVariable> _variables = new Dictionary<string, WSVariable>();
        [SerializeField]
        private Dictionary<string, WSFunctionVariable> _functionVariables = new Dictionary<string, WSFunctionVariable>();

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Dictionary<string, WSVariable> myVariables
        {
            get { return _variables; }
            set { _variables = value; }
        }
        public Dictionary<string, WSFunctionVariable> myFunctionVariables
        {
            get { return _functionVariables; }
            set { _functionVariables = value; }
        }
        public GameObject myGO
        {
            get { return null; }
        }
        public WorldStateData() { }
        public WSVariable AddVariable(WSVariable variable)
        {
            if (variable == null)
                return null;
            WSVariable ret = AddVariable(variable.name, variable.value.GetType());
            if (ret != null)
                ret.value = variable.value;

            return ret;
        }
        public WSVariable AddVariable(string variable_name, object value)
        {
            if (value == null)
                return null;
            WSVariable ret = AddVariable(variable_name, value.GetType());
            if (ret != null)
                ret.value = value;

            return ret;
        }
        public WSVariable AddVariable(string variable_name, Type type)
        {
            WSVariable ret = null;
            if (_variables.ContainsKey(variable_name))
            {
                ret = GetVariable(variable_name, type);
                if (ret == null)
                {
                    Debug.LogError("Variable exist but type is different returning null");
                }
                else
                {
                    Debug.LogWarning("Variable exist already returning copy");

                }
                return ret;
            }

            Type data_type = typeof(WSVariable<>).RHelperGenericType(new Type[] { type });
            ret = (WSVariable)Activator.CreateInstance(data_type);
            ret.name = variable_name;
            _variables[variable_name] = ret;
            if (onVariableAdded != null)
            {
                onVariableAdded(ret);
            }
            return ret;
        }
        public WSFunctionVariable AddFunctionVariable(WSFunctionVariable method,bool value)
        {
            if (method == null)
                return null;
            WSFunctionVariable ret = AddFunctionVariable(method.methodInfo, method.target.gameObject);
            if (ret != null)
                ret.value = value;           

            return ret;
        }
      
        public WSFunctionVariable AddFunctionVariable(MethodInfo value, GameObject agent)
        {
            WSFunctionVariable ret = null;
            if (_functionVariables.ContainsKey(value.Name))
            {
                ret = GetFunctionVariable(value.Name, value.DeclaringType);
                if (ret == null)
                {
                    Debug.LogError("Variable exist but type is different returning null");
                }
                else
                {
                    Debug.LogWarning("Variable exist already returning copy");

                }
                return ret;
            }
            ret = new WSFunctionVariable(value, agent.GetComponent< WSVariableParameter>());
            ret.name = value.Name;
            _functionVariables[value.Name] = ret;

            if (onFunctionVariableAdded != null)
            {
                onFunctionVariableAdded(ret);
            }
            return ret;
        }
        public WSVariable RemoveVariable(string variable_name)
        {
            WSVariable ret = null;
            if (_variables.TryGetValue(variable_name, out ret))
            {
                _variables.Remove(variable_name);
                if (onVariableRemoved != null)
                {
                    onVariableRemoved(ret);
                }
            }
            return ret;
        }
        public WSFunctionVariable RemoveFunctionVariable(string variable_name)
        {
            WSFunctionVariable ret = null;
            if (_functionVariables.TryGetValue(variable_name, out ret))
            {
                _functionVariables.Remove(variable_name);
                if (onFunctionVariableRemoved != null)
                {
                    onFunctionVariableRemoved(ret);
                }
            }
            return ret;
        }
        public WSVariable GetVariable(string variable_name, Type type = null)
        {
            if (_variables == null && variable_name == null)
                return null;
            WSVariable ret = null;
            if (_variables.TryGetValue(variable_name, out ret))
            {
                if (type == null || ret.CanConvertTo(type))
                    return ret;
            }
            return null;

        }
        public WSVariable GetVariableByID(string ID)
        {
            if (_variables == null && ID != null)
                return null;
            foreach (WSVariable it in _variables.Values)
            {
                if (it.ID == ID)
                    return it;
            }
            return null;
        }
        public WSFunctionVariable GetFunctionVariable(string variable_name, Type type = null)
        {
            if (_functionVariables == null && variable_name == null)
                return null;
            WSFunctionVariable ret = null;
            if (_functionVariables.TryGetValue(variable_name, out ret))
            {
                return ret;
            }
            return null;

        }
        public WSFunctionVariable GetFunctionVariableByID(string ID)
        {
            if (_functionVariables == null && ID != null)
                return null;
            foreach (WSFunctionVariable it in _functionVariables.Values)
            {
                if (it.ID == ID)
                    return it;
            }
            return null;
        }
        public WSVariable<T> GetVariable<T>(string variable_name)
        {
            return (WSVariable<T>)GetVariable(variable_name, typeof(T));

        }
        public T GetTValue<T>(string variable_name)
        {
            try
            {
                return (T)_variables[variable_name].value;
            }
            catch
            {
                if (_variables.ContainsKey(variable_name))
                {
                    //Exist
                    return default(T);
                }
            }
            //DontExist
            return default(T);


        }
        public bool Exist(WSVariable variable)
        {
            return _variables.ContainsValue(variable);
        }
        public bool Exist(string name)
        {
            return _variables.ContainsKey(name);
        }
        public void Clear()
        {
            _variables.Clear();
        }
        public WSVariable SetValue(string variable_name, object value)
        {
            if (_variables == null)
                return null;
            try
            {
                WSVariable ret = _variables[variable_name];
                ret.value = value;
                return ret;
            }
            catch
            {
                if (!_variables.ContainsKey(variable_name))
                {
                    WSVariable ret = AddVariable(variable_name, value);
                    ret.isProtected = true;
                    return ret;

                }
            }
            return null;
        }
        public string[] GetVariableNames()
        {
            if (_variables == null)
                return null;
            return _variables.Keys.ToArray();
        }
        public string[] GetFunctionVariableNames()
        {
            if (_functionVariables == null)
                return null;
            return _functionVariables.Keys.ToArray();
        }
        public string[] GetVariableNames(Type type)
        {
            return _variables.Values.Where(variable => variable.CanConvertTo(type)).Select(v => v.name).ToArray();

        }
        public void InitPropertyBinding(GameObject target, bool call_setter)
        {
            foreach (var data in _variables.Values)
            {
                data.InitializePropertyBinding(target, call_setter);
            }
        }
    }
}