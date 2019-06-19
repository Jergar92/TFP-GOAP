using System;
using System.Collections.Generic;
using UnityEngine;
using GOAP.Framework;

namespace GOAP.Framework.Internal
{
    [Serializable]
    public class GoapGraphSerializationData
    {


        public string _name;
        public string _comments;
        public List<NodeAction> _nodeAction;
        public List<NodeGoal> _nodeGoal;

        public GoapGraphSerializationData() { }
        public GoapGraphSerializationData(GoapGraph goapGraph)
        {
            _name = goapGraph.name;
            _comments = goapGraph.comments;
            _nodeAction = goapGraph.allNodeActions;
            _nodeGoal = goapGraph.allNodeGoals;
        }
        public void SetGraphToNode(GoapGraph goapGraph)
        {
            foreach (NodeAction action in _nodeAction)
            {
                action.goapGraph = goapGraph;
            }
            foreach (NodeGoal goal in _nodeGoal)
            {
                goal.goapGraph = goapGraph;

            }
        }
    }
}