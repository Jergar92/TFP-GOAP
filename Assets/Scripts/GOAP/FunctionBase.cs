using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Serialization;
using GOAP.Helper;
namespace GOAP.Framework
{
    [Serializable]
    public partial class FunctionBase
    {


        [AttributeUsage(AttributeTargets.Class)]
        protected class AgentTypeAttribute : Attribute

        {
            public Type type;
            public AgentTypeAttribute(Type type)
            {
                this.type = type;
            }
        }
        /*TODO LEARN THIS
        [AttributeUsage(AttributeTargets.Class)]
        protected class EventReceiverAttribute:Attribute
        {
            public string[] event_messages;
            public EventReceiverAttribute(params string [] args)
            {
                this.event_messages = args;
            }
        }
        */
        [AttributeUsage(AttributeTargets.Field)]
        protected class GetFromAgentAttribute : Attribute { }


        [Serializable]
        public class FunctionAgent : WSParameter<UnityEngine.Object>
        {
            new public UnityEngine.Object value
            {
                get
                {
                    if (useWorldState)
                    {
                        var obj = base.value;

                        if (obj == null)
                            return null;

                        else if (obj is GameObject)
                            return (obj as GameObject).transform;

                        else if (obj is Component)
                            return (Component)obj;

                        return null;
                    }

                    return _value as Component;
                }
                set
                {
                    _value = value;
                }
            }
            protected override object objectValue
            {
                get
                {
                    return this.value;
                }
                set
                {
                    this.value = (UnityEngine.Object)value;
                }
            }
        }

        [SerializeField]
        private bool _isActive;
        [SerializeField]
        private FunctionAgent override_agent = null;


        [NonSerialized]
        private IWorldState _worldState;
        [NonSerialized]
        private IFunctionSystem _ownerSystem;
        [NonSerialized]
        private Component _agent;
        [NonSerialized]
        private bool _isAgentTypeInit;
        [NonSerialized]
        private Type _agentType;
        [NonSerialized]
        private string _functionName;
        [NonSerialized]
        private string _functionDescription;



        public FunctionBase() { }
        public static FunctionBase Create(Type type, IFunctionSystem newOwnerSystem)
        {
            FunctionBase new_function = (FunctionBase)Activator.CreateInstance(type);

#if UNITY_EDITOR
#endif

            new_function.SetOwnerSystem(newOwnerSystem);
            WSParameter.SetWSFields(new_function, newOwnerSystem.worldState);
            new_function.OnValidate(newOwnerSystem);
            return new_function;
        }
        virtual public FunctionBase Duplicate(IFunctionSystem newOwnerSystem)
        {
            FunctionBase new_function = JSONSerializer.Deserialize<FunctionBase>(JSONSerializer.Serialize(typeof(FunctionBase), this));


            new_function.SetOwnerSystem(newOwnerSystem);
            WSParameter.SetWSFields(new_function, newOwnerSystem.worldState);
            new_function.OnValidate(newOwnerSystem);
            return new_function;
        }
        virtual public void OnValidate(IFunctionSystem newOwnerSystem)
        {
        }

        public void SetOwnerSystem(IFunctionSystem newOwnerSystem)
        {
            if (newOwnerSystem == null)
            {
                Debug.LogError("ERROR:New owner system is NULL");
                return;
            }
            ownerSystem = newOwnerSystem;

#if UNITY_EDITOR
            worldState = newOwnerSystem.worldState;
#endif
        }

        protected IWorldState worldState
        {
            get
            {
                return _worldState;
            }
            set
            {
                if (_worldState != value)
                {
                    _worldState = value;
                    WSParameter.SetWSFields(this, value);
                    if (override_agent != null)
                        override_agent.worldState = value;
                }
            }
        }
        virtual public Type agentType
        {
            get
            {
                if (!_isAgentTypeInit)
                {
                    var typeAttribute = GetType().RHelperGetAttribute<AgentTypeAttribute>(true);
                    if (typeAttribute != null)
                    {
                        if (!typeAttribute.type.RHelperIsInterface())
                        {
                            Debug.LogWarning("Don't use this for interfaces");
                        }
                        _agentType = typeAttribute.type;
                    }
                    _isAgentTypeInit = true;
                }
                return _agentType;
            }
        }
        public string summaryInfo
        {
            get
            {
                return info;
            }
        }
        virtual protected string info
        {
            get { return name; }
        }
        public string agentInfo
        {
            get { return override_agent != null ? override_agent.ToString() : "<b>owner</b>"; }
        }
        public string name
        {
            get
            {
                if (_functionName == null)
                {
                    NameAttribute name_attribute = GetType().RHelperGetAttribute<NameAttribute>(false);
                    _functionName = name_attribute != null ? name_attribute.name : GetType().FriendlyName();
                }
                return _functionName;
            }
        }
        protected Component agent
        {

            get
            {
                if (_agent != null)
                {
                    return _agent;
                }

                Component ret = agentIsOverride ? (Component)override_agent.value : ownerAgent;
                if (ret != null && agentType != null && (agentType.IsSubclassOf(typeof(Component)) || agentType.IsInterface))
                {
                    return ret.GetComponent(agentType);
                }

                return null;
            }
        }
        public bool agentIsOverride
        {
            get
            {
                return override_agent != null;
            }
            private set
            {
                if (value == false && override_agent != null)
                {
                    override_agent = null;
                }
                if (value == true && override_agent == null)
                {
                    override_agent = new FunctionAgent();
                }
            }
        }
        public bool isActive
        {
            get
            {
                return !_isActive;
            }
            set
            {
                _isActive = !value;
            }
        }
        public IFunctionSystem ownerSystem
        {
            get
            {
                return _ownerSystem;
            }
            private set
            {
                if (_ownerSystem != value)
                {
                    _ownerSystem = value;

                }
            }
        }
        private Component ownerAgent
        {
            get
            {
                return ownerSystem != null ? ownerSystem.agent : null;
            }
        }
        private IWorldState ownerWorldState
        {
            get
            {
                return ownerSystem != null ? ownerSystem.worldState : null;
            }
        }

        protected bool Set(Component new_agent, IWorldState newWorldState)
        {
            worldState = newWorldState;

            if (agentIsOverride)
                new_agent = override_agent.value as Component;

            if (_agent != null && new_agent != null && _agent.gameObject == new_agent.gameObject)
                return isActive = true;

            return isActive = Initialize(new_agent, agentType);
        }

        private bool Initialize(Component newAgent, Type newAttribute)
        {
            return true;
        }
        private bool InitializeAttribute(Component newAgent)
        {
            return true;
        }
        public string GetError()
        {
            //TODO Check errors
            return null;
        }

        public sealed override string ToString()
        {
            return string.Format("{0} {1}", agentInfo, summaryInfo);
        }
    }
}