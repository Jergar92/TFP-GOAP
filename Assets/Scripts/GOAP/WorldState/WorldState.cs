using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using FullSerializer;
using GOAP.Serialization;
using GOAP.Framework.Internal;

namespace GOAP.Framework
{
    public class WorldState : MonoBehaviour, IWorldState, ISerializationCallbackReceiver
    {


        public event Action<WSVariable> onVariableAdded;
        public event Action<WSVariable> onVariableRemoved;

        public event Action<WSFunctionVariable> onFunctionVariableAdded;
        public event Action<WSFunctionVariable> onFunctionVariableRemoved;
        [SerializeField]
        private string _serializedWorldState = null;
        [SerializeField]
        private List<UnityEngine.Object> _objectReferences = null;

        [NonSerialized]
        private WorldStateData _worldState = new WorldStateData();
        [NonSerialized]
        private bool isDeserialized = false;

        public void OnBeforeSerialize()
        {
            if (_objectReferences != null && _objectReferences.Any(o => o != null))
                isDeserialized = false;
#if UNITY_EDITOR
            if (JSONSerializer.appIsPlaying)
                return;
            _objectReferences = new List<UnityEngine.Object>();
            _serializedWorldState = JSONSerializer.Serialize(typeof(WorldStateData), _worldState, false, _objectReferences);
#endif
        }
        public void OnAfterDeserialize()
        {
            if (isDeserialized && JSONSerializer.appIsPlaying)
                return;
            isDeserialized = true;
            _worldState = JSONSerializer.Deserialize<WorldStateData>(_serializedWorldState, _objectReferences);
            if (_worldState == null)
                _worldState = new WorldStateData();
        }
        void Awake()
        {
            _worldState.InitPropertyBinding(myGO, false);
        }
        new public string name
        {
            get { return _worldState.name; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = gameObject.name + "_BB";
                }
                _worldState.name = value;
            }
        }
        public Dictionary<string, WSVariable> myVariables
        {
            get { return _worldState.myVariables; }
            set { _worldState.myVariables = value; }
        }
        public Dictionary<string, WSFunctionVariable> myFunctionVariables
        {
            get { return _worldState.myFunctionVariables; }
            set { _worldState.myFunctionVariables = value; }
        }
        public GameObject myGO
        {
            get { return gameObject; }
        }
        public WSVariable AddVariable(WSVariable variable)
        {
            return _worldState.AddVariable(variable);
        }
        public WSVariable AddVariable(string name, Type type)
        {
            WSVariable ret = _worldState.AddVariable(name, type);
            if (onVariableAdded != null)
                onVariableAdded(ret);
            return ret;
        }
        public WSFunctionVariable AddFunctionVariable(WSFunctionVariable variable, bool value)
        {
            return _worldState.AddFunctionVariable(variable, value);
        }
    
        public WSFunctionVariable AddFunctionVariable(MethodInfo value, GameObject agent)
        {
            WSFunctionVariable ret = _worldState.AddFunctionVariable(value, myGO);
            if (onFunctionVariableAdded != null)
                onFunctionVariableAdded(ret);
            return ret;
        }
        public WSVariable RemoveVariable(string name)
        {
            WSVariable ret = _worldState.RemoveVariable(name);
            if (onVariableRemoved != null)
                onVariableRemoved(ret);
            return ret;

        }
        public WSFunctionVariable RemoveFunctionVariable(string name)
        {
            WSFunctionVariable ret = _worldState.RemoveFunctionVariable(name);
            if (onFunctionVariableRemoved != null)
                onFunctionVariableRemoved(ret);
            return ret;

        }
        public WSVariable GetVariable(string name, Type type = null)
        {
            return _worldState.GetVariable(name, type);
        }
        public WSVariable GetVariableByID(string ID)
        {
            return _worldState.GetVariableByID(ID);

        }
        public WSFunctionVariable GetFunctionVariable(string name, Type type = null)
        {
            return _worldState.GetFunctionVariable(name, type);
        }
        public WSFunctionVariable GetFunctionVariableByID(string ID)
        {
            return _worldState.GetFunctionVariableByID(ID);

        }
        public WSVariable<T> GetVariable<T>(string name)
        {
            return _worldState.GetVariable<T>(name);

        }
        public T GetTValue<T>(string name)
        {
            return _worldState.GetTValue<T>(name);

        }
        public bool Exist(WSVariable variable)
        {
            return _worldState.Exist(variable);
        }
        public bool Exist(string name)
        {
            return _worldState.Exist(name);
        }
        public void Clear()
        {
            _worldState.Clear();
        }
        public WSVariable SetValue(string name, object value)
        {
            return _worldState.SetValue(name, value);

        }
        public string[] GetVariableNames()
        {
            return _worldState.GetVariableNames();

        }
        public string[] GetFunctionVariableNames()
        {
            return _worldState.GetFunctionVariableNames();

        }
        public string[] GetVariableNames(Type type)
        {
            return _worldState.GetVariableNames(type);

        }
    }
}