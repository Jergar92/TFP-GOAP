using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using GOAP.Helper;
namespace GOAP.Framework
{
    abstract public class WSVariable
    {

        [SerializeField]
        private string _name;
        [SerializeField]
        private string _id;
        [SerializeField]
        private bool _variableProtected;

        public event Action<string> on_name_changed;
        public event Action<string, object> on_value_changed;
        public string name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    if (on_name_changed != null)
                        on_name_changed(value);
                }
            }
        }
        public string ID
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    _id = Guid.NewGuid().ToString();

                }
                return _id;
            }
        }
        public bool isProtected
        {
            get { return _variableProtected; }
            set { _variableProtected = value; }
        }
        public object value
        {
            get { return objectValue; }
            set { objectValue = value; }
        }

        protected bool HasValueChangeEvent()
        {
            return on_value_changed != null;
        }

        protected void OnValueChanged(string name, object value)
        {
            on_value_changed(name, value);
        }
        public WSVariable() { }

        abstract protected object objectValue { get; set; }
        abstract public Type myType { get; }
        abstract public string propertyPath { get; set; }
        abstract public bool isBinded { get; }
        abstract public void BindProperty(MemberInfo prop, GameObject target = null);
        abstract public void UnBindProperty();

        abstract public void InitializePropertyBinding(GameObject go, bool use_setter = false);

        public bool CanConvertTo(Type type) { return GetGetConverter(type) != null; }

        public Func<object> GetGetConverter(Type type)
        {
            if (type.RHelperIsAssignableFrom(myType))
            {
                return () => { return value; };

            }
            if (type == typeof(Transform) && myType == typeof(GameObject))
            {
                return () => { return value != null ? (value as GameObject).transform : null; };
            }
            if (type == typeof(GameObject) && typeof(Component).RHelperIsAssignableFrom(myType))
            {
                return () => { return value != null ? (value as Component).gameObject : null; };
            }
            return null;

        }


        public bool CanConvertFrom(Type type) { return GetSetConverter(type) != null; }

        public Action<object> GetSetConverter(Type type)
        {
            if (type.RHelperIsAssignableFrom(myType))
            {
                return (obj) => { value = obj; };

            }
            if (type == typeof(Transform) && myType == typeof(GameObject))
            {
                return (obj) => { value = obj != null ? (obj as GameObject).transform : null; };
            }
            if (type == typeof(GameObject) && typeof(Component).RHelperIsAssignableFrom(myType))
            {
                return (obj) => { value = obj != null ? (obj as Component).gameObject : null; };
            }
            return null;

        }
        public override string ToString()
        {
            return name;
        }
    }

    [Serializable]
    public class WSVariable<T> : WSVariable
    {
        [SerializeField]
        protected T _value;

        [SerializeField]
        private string property_path;

        public WSVariable() { }

        private Func<T> getter;
        private Action<T> setter;

        public override string propertyPath
        {
            get { return property_path; }
            set
            {
                if (property_path != value)
                    property_path = value;
            }
        }
        public override bool isBinded
        {
            get
            {
                return !string.IsNullOrEmpty(propertyPath);
            }
        }
        protected override object objectValue
        {
            get { return value; }
            set { this.value = (T)value; }
        }
        public override Type myType
        {
            get { return typeof(T); }
        }
        new public T value
        {
            get { return getter != null ? getter() : _value; }
            set
            {
                if (base.HasValueChangeEvent())
                {
                    if (!Equals(_value, value))
                    {
                        _value = value;
                        if (setter != null)
                            setter(value);
                        OnValueChanged(name, value);
                    }
                    return;
                }
                if (setter != null)
                    setter(value);
                else
                    _value = value;

            }
        }
        public T GetValue()
        {
            return _value;
        }
        public void SetValue(T new_value)
        {
            value = new_value;
        }
        public override void BindProperty(MemberInfo prop, GameObject target = null)
        {
            if (prop is PropertyInfo || prop is FieldInfo)
            {
                propertyPath = string.Format("{0}.{1}", prop.RHelperReflectedType().FullName, prop.Name);
                if (target != null)
                {
                    InitializePropertyBinding(target, false);
                }
            }
        }
        public override void UnBindProperty()
        {
            propertyPath = null;
            getter = null;
            setter = null;
        }
        public override void InitializePropertyBinding(GameObject go, bool use_setter = false)
        {
            if (!isBinded || !Application.isPlaying)
                return;

            getter = null;
            setter = null;

            int index = propertyPath.LastIndexOf('.');
            string type_str = propertyPath.Substring(0, index);
            string member_str = propertyPath.Substring(index + 1);
            Type type = ReleflectionHelper.GetType(type_str);

            if (type == null)
            {
                Debug.LogError(string.Format("Type '{0}' not found", type_str));
                return;
            }
            PropertyInfo property = type.RHelperGetPropertyInfo(member_str);
            if (property != null)
            {
                MethodInfo get_method = property.RHelperGetGetMethodInfo();
                MethodInfo set_method = property.RHelperGetSetMethodInfo();
                bool is_static = (get_method != null && get_method.IsStatic) || (set_method != null && set_method.IsStatic);
                Component instance = is_static ? null : go.GetComponent(type);
                if (instance == null && !is_static)
                {
                    Debug.LogError(string.Format("A World State variable '{0}' is try to bind a component type '{1}', is missing binding ignored", name, type_str));
                    return;
                }

                if (property.CanRead)
                {
                    try
                    {
                        getter = get_method.RHelperCreateDelegate<Func<T>>(instance);
                    }
                    catch
                    {
                        getter = () => { return (T)get_method.Invoke(instance, null); };
                    }
                }
                else
                {
                    getter = () =>
                    {
                        Debug.LogError(string.Format("You try to Get a the variable '{0}' but this property '{1}' is write only", name, propertyPath));
                        return default(T);
                    };
                }

                if (property.CanWrite)
                {
                    try
                    {
                        setter = set_method.RHelperCreateDelegate<Action<T>>(instance);
                    }
                    catch
                    {
                        setter = (o) => { set_method.Invoke(instance, new object[] { o }); };
                    }

                    if (use_setter)
                        setter(_value);

                }
                else
                {
                    Debug.LogError(string.Format("You try to set a the variable '{0}' but this property '{1}' is Read only", name, propertyPath));

                }

            }
            FieldInfo field = type.RHelperGetFieldInfo(member_str);
            if (field != null)
            {
                Component instance = field.IsStatic ? null : go.GetComponent(type);
                if (instance == null && !field.IsStatic)
                {
                    Debug.LogError(string.Format("A World State variable '{0}' is try to bind a component type '{1}', is missing binding ignored", name, type_str));
                    return;
                }
                if (field.RHelperIsFieldReadOnly())
                {
                    T value = (T)field.GetValue(instance);
                    getter = () => { return value; };
                }
                else
                {
                    getter = () => { return (T)field.GetValue(instance); };
                    setter = (o) => { field.SetValue(instance, o); };
                }
                return;
            }
            Debug.LogError(string.Format("A World State variable '{0}' is try to bind a property or a field named '{1}', is does not exist on '{2}' binding ignored", name, member_str, type.FullName));

        }
    }

    [Serializable]
    public class WSFunctionVariable
    {

        public WSFunctionVariable() { }

        public WSFunctionVariable(MethodInfo method, WSVariableParameter intance)
        {
          //  _methodInfo = method;

            _methodName = method.Name;
            _methodType = method.DeclaringType;
            if (method.IsPrivate)
                _flags |= (int)BindingFlags.NonPublic;
            else
                _flags |= (int)BindingFlags.Public;
            if (method.IsStatic)
                _flags |= (int)BindingFlags.Static;
            else
                _flags |= (int)BindingFlags.Instance;

            _instance = intance;
        }

        [SerializeField]
        private string _name;
        [SerializeField]
        private string _id;

        public event Action<string> on_name_changed;

        //[SerializeField]
       // private MethodInfo _methodInfo=null;
        [SerializeField]
        private Type _methodType;
        [SerializeField]
        private string _methodName;
        [SerializeField]
        private int _flags = 0;
        [SerializeField]
        private WSVariableParameter _instance = null;

        private bool methodValue = false;
        private object[] _arguments;
        public string name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    if (on_name_changed != null)
                        on_name_changed(value);

                }
            }
        }
        public string ID
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    _id = Guid.NewGuid().ToString();

                }
                return _id;
            }
        }
        public bool value
        {
            get { return methodValue; }
            set { methodValue = value; }
        }
        public object[] arguments
        {
            get { return _arguments; }
            set { _arguments = value; }
        }
        public MethodInfo methodInfo
        {
            get
            {   if(_methodType ==null)
                {
                    return null;
                }
                return _methodType.GetMethod(_methodName, (BindingFlags)_flags);
            }
          
        }  
        public WSVariableParameter target
        {
            get
            {
                return _instance;
            }
        }

        public override string ToString()
        {
            return name;
        }
    }

    ///This is a very special dummy struct for variable header separators
    public struct WSSeparator
    {
#if UNITY_EDITOR
        [NonSerialized]
        public bool isEditingName;
#endif
    }
}