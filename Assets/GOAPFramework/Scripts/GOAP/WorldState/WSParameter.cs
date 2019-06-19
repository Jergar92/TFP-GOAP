using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using GOAP.Helper;



namespace GOAP.Framework
{
    [AttributeUsage(AttributeTargets.Field)]
    public class WorldStateAttribute : Attribute { }

    [Serializable]
    public abstract class WSParameter
    {

        [SerializeField]
        private string _name;
        [SerializeField]
        private string ws_variable_id;

        [NonSerialized]
        private IWorldState _worldState;
        [NonSerialized]
        private WSVariable variable_reference;


        //TODO:CHECK LATER
        public WSParameter() { }

        abstract public Type variableType { get; }
        abstract protected object objectValue { get; set; }

        abstract protected void Bind(WSVariable data);
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    variable_reference = value != null ? ResolveReference(_worldState, false) : null;
                }
                else
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        if (variable_reference == null && !string.IsNullOrEmpty(value))
                        {
                            variable_reference = ResolveReference(_worldState, false);
                        }
                    }
#endif
                }
            }
        }
        public bool useWorldState
        {
            get
            {
                return name != null;
            }
            set
            {
                if (value == false)
                    name = null;
                if (value == true && name == null)
                    name = string.Empty;
            }
        }
        public bool isNone
        {
            get
            {
                return name == string.Empty;
            }
        }
        public bool isNull
        {
            get
            {
                return Equals(objectValue, null);
            }
        }
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
        public override string ToString()
        {
            if (isNone)
            {
                return "<b>NONE</b>";
            }
            if (useWorldState)
            {
                string ret = string.Format("<b>${0}</b>", name);

                return ret;
            }
            if (isNull)
                return "<b>NULL</b>";
            return string.Format("<b>{0}</b>", objectValue.ToStringAdvanced());

        }
        public static WSParameter CreateInstance(Type type, IWorldState worldState)
        {
            if (type == null)
                return null;

            WSParameter newWSParameter = (WSParameter)Activator.CreateInstance(typeof(WSParameter<>).RHelperGenericType(new Type[] { type }));
            newWSParameter.worldState = worldState;

            return newWSParameter;
        }
        public static void SetWSFields(object obj, IWorldState worldState)
        {
            List<WSParameter> wsParameter = GetObjectWSParameters(obj);
            for (int i = 0; i < wsParameter.Count; i++)
            {
                wsParameter[i].worldState = worldState;
            }
        }
        public static List<WSParameter> GetObjectWSParameters(object obj)
        {
            List<WSParameter> retParameter = new List<WSParameter>();
            FieldInfo[] fields = obj.GetType().RHelperGetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                if (typeof(WSParameter).RHelperIsAssignableFrom(field.FieldType))
                {
                    var value = field.GetValue(obj);
                    if (value == null && field.FieldType != typeof(WSParameter))
                    {
                        value = Activator.CreateInstance(field.FieldType);
                        field.SetValue(obj, value);
                    }
                    if (value != null)
                        retParameter.Add((WSParameter)value);
                }
            }
            return retParameter;
        }
        public WSVariable variableReference
        {
            get
            {
                return variable_reference;
            }
            set
            {
                if (variable_reference != value)
                {
                    if (variable_reference != null)
                    {
                        variable_reference.on_name_changed -= OnReferenceNameChanged;
                    }
                    if (value != null)
                    {
                        value.on_name_changed += OnReferenceNameChanged;
                        OnReferenceNameChanged(value.name);
                    }
                    variable_reference = value;
                    ws_variable_id = value != null ? value.ID : null;
                    Bind(value);
                }
            }
        }
        void OnReferenceNameChanged(string new_name)
        {
            name = new_name;
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
        public void OnWSVariableAdded(WSVariable variable)
        {
            if (variable.name == this.name && variable.CanConvertTo(variableType))
                variableReference = variable;
        }
        public void OnWSVariableRemoved(WSVariable variable)
        {
            if (variable == variableReference)
                variableReference = null;
        }
        public WSVariable PromoteToVariable(IWorldState targetWS)
        {
            if (string.IsNullOrEmpty(name))
            {
                variable_reference = null;
                return null;
            }
            string var_name = name;
            string ws_name = targetWS != null ? targetWS.name : string.Empty;

            if (targetWS == null)
            {
                variable_reference = null;
                Debug.LogError(string.Format("Parameter '{0}', world state with name'{1}' not found, fail to promote to variable", var_name, ws_name));
                return null;
            }

            variable_reference = targetWS.AddVariable(var_name, variableType);
            Debug.Log(string.Format("Parameter '{0}' with type '{1}' promote to variable", var_name, variableType.FriendlyName()));
            return variable_reference;
        }
        private WSVariable ResolveReference(IWorldState target__worldState, bool use_id)
        {
            string target_name = name;
            if (target_name != null && target_name.Contains("/"))
            {
                string[] split = target_name.Split('/');

            }
            WSVariable ret = null;
            if (target__worldState == null)
                return null;
            if (use_id && ws_variable_id != null)
                ret = target__worldState.GetVariableByID(ws_variable_id);
            if (ret == null && !string.IsNullOrEmpty(target_name))
            {
                ret = target__worldState.GetVariable(target_name, variableType);
            }
            return ret;
        }
        public WSVariable ResolveReference(IWorldState target__worldState, string referenceName)
        {
            if (string.IsNullOrEmpty(referenceName))
                return null;
            string target_name = name;
            if (target_name != null && target_name.Contains("/"))
            {
                string[] split = target_name.Split('/');

            }
            WSVariable ret = null;
            if (target__worldState == null)
                return null; 
            if (ret == null && !string.IsNullOrEmpty(target_name))
            {
                ret = target__worldState.GetVariable(target_name, variableType);
            }
            return ret;
        }

    }

    [Serializable]
    public class WSParameter<T> : WSParameter
    {
        public WSParameter() { }
        public WSParameter(T value) { _value = value; }

        private Func<T> getter;
        private Action<T> setter;

        [SerializeField]
        protected T _value;

        new public T value
        {
            get
            {
                if (getter != null)
                    return getter();

                if (Application.isPlaying && !string.IsNullOrEmpty(name) && worldState != null)
                {
                    variableReference = PromoteToVariable(worldState);

                    return getter != null ? getter() : default(T);
                }

                return _value;
            }
            set
            {
                if (setter != null)
                {
                    setter(value);
                    return;
                }

                if (isNone)
                    return;

                if (!string.IsNullOrEmpty(name) && worldState != null)
                {
                    variableReference = PromoteToVariable(worldState);

                    if (setter != null)
                        setter(value);

                    return;
                }
                _value = value;
            }

        }

        protected override object objectValue
        {
            get { return value; }
            set { this.value = (T)value; }
        }
        public override Type variableType
        {
            get
            {
                return typeof(T);
            }
        }

        protected override void Bind(WSVariable variable)
        {
            if (variable == null)
            {
                getter = null;
                setter = null;
                _value = default(T);
                return;
            }
            BindGetter(variable);
            BindSetter(variable);
        }
        bool BindGetter(WSVariable variable)
        {
            if (variable is WSVariable<T>)
            {
                getter = (variable as WSVariable<T>).GetValue;
                return true;
            }
            if (variable.CanConvertTo(variableType))
            {
                var function = variable.GetGetConverter(variableType);
                getter = () => { return (T)function(); };
                return true;
            }
            return false;
        }
        bool BindSetter(WSVariable variable)
        {
            if (variable is WSVariable<T>)
            {
                setter = (variable as WSVariable<T>).SetValue;
                return true;
            }
            if (variable.CanConvertFrom(variableType))
            {
                var func = variable.GetSetConverter(variableType);
                setter = (T value) => { func(value); };
                return true;
            }
            return false;
        }

        public static implicit operator WSParameter<T>(T value)
        {
            return new WSParameter<T> { value = value };
        }
    }
}