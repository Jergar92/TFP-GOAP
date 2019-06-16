using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GOAP.Framework
{
    public class GoapOwner : MonoBehaviour
    {

        public enum EnableGOAP
        {
            ENABLE,
            DO_NOTHING
        }
        public enum DisableGOAP
        {
            DISABLE,
            PAUSE,
            DO_NOTHING
        }
        [HideInInspector]
        public EnableGOAP enable_action = EnableGOAP.ENABLE;
        [HideInInspector]
        public DisableGOAP disable_action = DisableGOAP.DISABLE;

        [SerializeField]
        [HideInInspector]
        private GoapGraph goap_graph = null;
        [SerializeField]
        [HideInInspector]
        private WorldState world_state = null;
        [SerializeField]
        [HideInInspector]
        private WSVariableParameter WSVariableParameter = null;
        private bool goap_start = false;
        public static bool app_quit;
        // public System.Type graphType { get { return typeof(GoapGraph); } }
        public IWorldState worldState
        {
            get
            {
                if (world_state == null)
                {
                    world_state = GetComponent<WorldState>();
                }
                return world_state;
            }
            set
            {
                if (world_state != (object)value)
                {
                    world_state = (WorldState)(object)value;
                    if (goapGraph != null)
                    {
                        goapGraph.worldState = value;
                    }
                }
            }
        }
        public GoapGraph goapGraph
        {
            get
            {
                return goap_graph;
            }
            set
            {
                if (goap_graph != value)
                {
                    goap_graph = GetInstance(value);
                }


            }
        }
        private void OnApplicationQuit()
        {
            app_quit = true;
        }
        private void Awake()
        {
            goapGraph = GetInstance(goapGraph);
        }
        private void OnEnable()
        {
            if (goap_start && enable_action == EnableGOAP.ENABLE)
                StartGoap();
        }
        private void OnDisable()
        {
            if (app_quit)
                return;

            if (disable_action == DisableGOAP.DISABLE)
                StopGoap();
            if (disable_action == DisableGOAP.PAUSE)
                PauseGoap();
        }
        private void OnDestroy()
        {
            if (app_quit)
                return;
            StopGoap();
        }
        private void Start()
        {
            goap_start = true;
            if (enable_action == EnableGOAP.ENABLE)
                StartGoap();
        }
        private void Update()
        {
            UpdateGoap();
        }
        public void StartGoap()
        {
            goapGraph = GetInstance(goapGraph);
            if (goap_graph != null)
                goap_graph.StartGoap(this, worldState);
        }

        public void UpdateGoap()
        {
            if (goap_graph != null)
                goap_graph.UpdateGoap();

        }
        public void StopGoap()
        {
            // if (goap_graph != null)
            //     goap_graph.StopGoap();
        }
        public void PauseGoap()
        {
            //  if (goap_graph != null)
            //      goap_graph.PauseGoap();
        }
        private GoapGraph GetInstance(GoapGraph original)
        {
            if (original == null)
                return null;
            if (!Application.isPlaying)
                return original;

            return Instantiate(original);
        }
        public bool SetGoalDesire(string goalName, int value)
        {
            if(!goap_graph.allNodeGoals.Exists(x=>x.name==goalName))
                return false;
            NodeGoal goal = goap_graph.allNodeGoals.Find(x => x.name == goalName);
            goal.goalPriority = value;
            return true;
        }
        public bool AddGoalDesire(string goalName, int value)
        {
            if (!goap_graph.allNodeGoals.Exists(x => x.name == goalName))
                return false;
            NodeGoal goal = goap_graph.allNodeGoals.Find(x => x.name == goalName);
            goal.goalPriority += value;
            return true;
        }
#if UNITY_EDITOR

        protected void Reset()
        {
            worldState = gameObject.GetComponent<WorldState>();
            if (world_state == null)
            {
                worldState = gameObject.AddComponent<WorldState>();
            }

            WSVariableParameter = gameObject.GetComponent<WSVariableParameter>();
            if (WSVariableParameter == null)
            {
                WSVariableParameter = gameObject.AddComponent<WSVariableParameter>();
            }
   
    }
   
#endif
}
}