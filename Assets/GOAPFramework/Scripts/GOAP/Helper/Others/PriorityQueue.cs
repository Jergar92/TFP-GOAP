using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using GOAP.Framework;
namespace GOAP.Helper
{
    public class PriorityQueue
    {

        private List<PlannerNode> _data;
        public int Count
        {
            get { return _data.Count; }
        }
        public PriorityQueue()
        {
            _data = new List<PlannerNode>();
        }

        public void Push(PlannerNode item)
        {
            _data.Add(item);
            int index = _data.Count - 1;
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (_data[index].CompareTo(_data[parentIndex]) <= 0)
                {
                    break;
                }
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }
        public PlannerNode Pull()
        {
            int lastIndex = _data.Count - 1;
            ShowQueue();
            PlannerNode ret = _data[0];
            ShowNode(ret);

            _data[0] = _data[lastIndex];

            _data.RemoveAt(lastIndex);

            int minIndex = 0;

            int parentIndex = 0;
            while (true)
            {
                
                parentIndex = minIndex;
                int leftIndex = parentIndex * 2+1;
                int rightIndex = leftIndex + 1;

                if (leftIndex < _data.Count && _data[leftIndex].CompareTo(_data[minIndex])==1)
                    minIndex = leftIndex;

                if (rightIndex <_data.Count && _data[rightIndex].CompareTo(_data[minIndex]) == 1)
                    minIndex = rightIndex;
              
                if (parentIndex == minIndex)
                    break;

                Swap(parentIndex, minIndex);
                parentIndex = minIndex;
            }

            foreach(PlannerNode node in _data)
            {
                if (node.node is NodeAction)
                {
                    NodeAction action = node.node as NodeAction;
                }
            }
            ShowQueue();

            return ret;
        }
       void ShowQueue()
        {
            foreach (PlannerNode node in _data)
            {
                if (node.node is NodeGoal)
                {
                    NodeGoal goal = node.node as NodeGoal;
                }
                if (node.node is NodeAction)
                {
                    NodeAction action = node.node as NodeAction;
                }
            }
        }
        void ShowNode(PlannerNode node)
        {
            
                if (node.node is NodeGoal)
                {
                    NodeGoal goal = node.node as NodeGoal;
                }
                if (node.node is NodeAction)
                {
                    NodeAction action = node.node as NodeAction;
                }
            
        }
        public PlannerNode this [int key]
        {
            get { return _data[key]; }
        }
        public void Clear()
        {
            _data.Clear();
        }
        public void RemoveAt(int index)
        {
            Swap(0, index);
            Pull();
        }
        public bool Contains(NodeAction item)
        {
            return _data.Any((node) => node.node == item);
        }
        public int FindIndex(NodeAction item)
        {
            return _data.FindIndex(node => node.node == item);
        }
        private void Swap(int a, int b)
        {
            PlannerNode tmpItem = _data[a];
            _data[a] = _data[b];
            _data[b] = tmpItem;
        }

    }
}
