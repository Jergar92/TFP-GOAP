using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using GOAP.Framework.Internal;
using GOAP.Helper;

namespace GOAP.Framework
{
    [Serializable]
    abstract public class PEParameter
    {

        [SerializeField]
        private string _name;
        [SerializeField]
        private string WSVariableID;

        [NonSerialized]
        private IWorldState _worldState;
        [NonSerialized]
        private WSVariable variable_reference;

        private Operator _myOperator = new Operator();

        public object value
        {
            get
            {
                return objectValue;
            }
            set
            {
                objectValue = value;
            }
        }
        abstract protected object objectValue { get; set; }
        abstract public Type variableType { get; }

        //TODO:CHECK LATER
        public PEParameter() { }


        public string name
        {

            get
            {
                if (variable_reference != null)
                {
                    if (_name != variable_reference.name)
                        _name = variable_reference.name;
                    return _name;
                }
                if (!string.IsNullOrEmpty(_name))
                {
                    return _name;
                }
                return null;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;

                }

            }
        }
        public IWorldState worldState
        {
            get
            {
                return _worldState;
            }
            set
            {
                if (_worldState != value)
                {
                    if (_worldState != null)
                    {
                        _worldState.onVariableAdded -= OnWSVariableAdded;
                        _worldState.onVariableRemoved -= OnWSVariableRemoved;
                    }
                    if (value != null)
                    {
                        value.onVariableAdded += OnWSVariableAdded;
                        value.onVariableRemoved += OnWSVariableRemoved;
                    }
                    _worldState = value;
                    variableReference = value != null ? ResolveReference(_worldState, true) : null;

                }
            }
        }
        void OnWSVariableAdded(WSVariable variable)
        {
            if (variable.name == this.name && variable.CanConvertTo(variableType))
                variableReference = variable;
        }
        void OnWSVariableRemoved(WSVariable variable)
        {
            if (variable == variableReference)
                variableReference = null;
        }

        public WSVariable variableReference
        {
            get
            {
                if (isValid)
                    return variable_reference;
                return null;
            }
            set
            {
                if (variableReference != value)
                {
                    variable_reference = value;
                    name = variable_reference != null ? variable_reference.name : null;
                }
            }
        }
        public bool isValid
        {
            get
            {
                if (variable_reference == null || string.IsNullOrEmpty(name))
                {
                    return false;
                }
                return true;
            }
        }
        public bool isNone
        {
            get
            {
                return name == string.Empty;
            }
        }
        public string operatorSymbol
        {
            get
            {
                return _myOperator.symbol;
            }
        }
        public Operator myOperator
        {
            get
            {
                return _myOperator;
            }
        }
        public bool DoOperator()
        {
            return true;
        }
        private WSVariable ResolveReference(IWorldState target_world_state, bool use_id)
        {
            string target_name = name;
            if (target_name != null && target_name.Contains("/"))
            {
                string[] split = target_name.Split('/');

            }
            WSVariable ret = null;
            if (target_world_state == null)
                return null;
            if (use_id && WSVariableID != null)
                ret = target_world_state.GetVariableByID(WSVariableID);
            if (ret == null && !string.IsNullOrEmpty(target_name))
            {
                ret = target_world_state.GetVariable(target_name, variableType);
            }
            return ret;
        }
    }
    [Serializable]
    public class PEParameter<T> : PEParameter
    {
        [SerializeField]
        private T _value;


      
        protected override object objectValue
        {
            get
            {
                return value;
            }
            set
            {
                this.value = (T)value;
            }
        }
        new public T value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        public override Type variableType
        {
            get
            {
                return typeof(T);
            }
        }
        public T GetValue()
        {
            return value;
        }
        public void SetValue(T new_value)
        {
            value = new_value;
        }
    }

    [Serializable]
    public class PEMethod
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _WSMethodID;

        [SerializeField]
        private bool _value;
 

        [SerializeField]
        private List<WSParameter> _arguments = new List<WSParameter>();

        [NonSerialized]
        private WSFunctionVariable _methodReference = null;
        [NonSerialized]
        private IWorldState _worldState;
        private Operator _myOperator = new Operator();

        public string name
        {

            get
            {
                if (_methodReference != null)
                {
                    if (_name != _methodReference.name)
                        _name = _methodReference.name;
                    return _name;
                }
                if (!string.IsNullOrEmpty(_name))
                {
                    return _name;
                }
                return null;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;

                }

            }
        }
        public Operator myOperator
        {
            get
            {
                return _myOperator;
            }
        }

        public WSFunctionVariable methodReference
        {
            get
            {
                return _methodReference;
            }
            set
            {
                if(_methodReference!=value)
                {
                    _methodReference = value;
                    var parameters = _methodReference.methodInfo.GetParameters();
                    if (parameters.Length != arguments.Count)
                    {
                        GenerateParameters(parameters);
                    }
                    else
                    {
                        if(!ParametersEquals(parameters))
                        {
                            GenerateParameters(parameters);
                        }
                    }                    
                }
            }
        }
        void GenerateParameters(ParameterInfo[] parameters)
        {
            arguments.Clear();
            foreach (ParameterInfo parameter in parameters)
            {
                var data_type = typeof(WSParameter<>).RHelperGenericType(new Type[] { parameter.ParameterType });
                var newData = (WSParameter)Activator.CreateInstance(data_type);

                _arguments.Add(newData);
            }
        }
        bool ParametersEquals(ParameterInfo[] parameters)
        {
            for(int i =0;i< parameters.Length;i++)
            {
               if(!parameters[i].ParameterType.Equals(arguments[i].variableType))
                {
                    return false;
                }
            }
            return true;
        }
        protected bool objectValue
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }
        public bool value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        public bool isValid
        {
            get
            {
                if (methodReference == null || string.IsNullOrEmpty(name))
                {
                    return false;
                }
                return true;
            }
        }
        public IWorldState worldState
        {
            get
            {
                return _worldState;
            }
            set
            {
                if (_worldState != value)
                {
                    if (_worldState != null)
                    {
                        _worldState.onFunctionVariableAdded -= OnWSFunctionVariableAdded;
                        _worldState.onFunctionVariableRemoved -= OnWSFunctionVariableRemoved;
                    }
                    if (value != null)
                    {
                        value.onFunctionVariableAdded += OnWSFunctionVariableAdded;
                        value.onFunctionVariableRemoved += OnWSFunctionVariableRemoved;
                    }
                    _worldState = value;
                    methodReference = value != null ? ResolveReference(_worldState, true) : null;
                    foreach(var item in arguments)
                    {
                        item.worldState = _worldState;
                    }
                    
                }
            }
        }
          public List<WSParameter> arguments
        {
            get
            {
                return _arguments;
            }
        }
        private WSVariableParameter target
        {
            get
            {
                return methodReference.target;
            }
        }
        void OnWSFunctionVariableAdded(WSFunctionVariable method)
        {
            if (method.name == this.name)
                methodReference = method;
        }
        void OnWSFunctionVariableRemoved(WSFunctionVariable method)
        {
            if (method == methodReference)
                methodReference = null;
        }
        public bool InvokeMethod()
        {
            return (bool)_methodReference.methodInfo.Invoke(target, GetArguments());
        }
        private WSFunctionVariable ResolveReference(IWorldState targetWorldState, bool use_id)
        {
            string targetName = name;
            if (targetName != null && targetName.Contains("/"))
            {
                string[] split = targetName.Split('/');

            }
            WSFunctionVariable ret = null;
            if (targetWorldState == null)
                return null;
            if (use_id && _WSMethodID != null)
                ret = targetWorldState.GetFunctionVariableByID(_WSMethodID);
            if (ret == null && !string.IsNullOrEmpty(targetName))
            {
                ret = targetWorldState.GetFunctionVariable(targetName);
            }
            return ret;
        }
        private WSVariable ResolveVariableReference(IWorldState targetWorldState, bool use_id,int index)
        {
            string targetName = name;
            if (targetName != null && targetName.Contains("/"))
            {
                string[] split = targetName.Split('/');

            }
            WSVariable ret = null;
            if (targetWorldState == null)
                return null;
            if (use_id && _WSMethodID != null)
                ret = targetWorldState.GetVariableByID(_WSMethodID);
            if (ret == null && !string.IsNullOrEmpty(targetName))
            {
                ret = targetWorldState.GetVariable(targetName);
            }
            return ret;
        }

        public bool IsMethodValid()
        {
            return value == InvokeMethod();
        }
        public object[] GetArguments()
        {
       
            if (arguments.Count < 0)
                return null;
            object[] param = new object[arguments.Count];
            for(int i = 0;i<arguments.Count;i++)
            {
                param[i] = arguments[i].value;
            }
            return param;
        }
    }
}