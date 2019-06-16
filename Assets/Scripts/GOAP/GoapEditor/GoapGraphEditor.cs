#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace GOAP.Framework
{
    partial class GoapGraph
    {    //World State Rect
        private Rect _worldStateRect = new Rect(15, 55, 0, 0);
        private GUILayoutOption[] _menuBarOptions;
        private readonly string[] selStrings = new string[] { "Editor Window", "Planning View" };
        private readonly float _worldStateSpacing = 20.0f;
        private readonly float _worldStateBaseWidth = 330.0f;
        private readonly float _worldStateBaseHeight = 30.0f;
        private readonly float _scrollSize = 30.0f;

        private readonly string title = "World State";
        private Vector2 _worldStateScrollPos;
        private Vector2 _plannerScrollPos;

        private Vector2 _actionScrollPos;
        private Vector2 _goalScrollPos;

        private Vector2 _priorityGoalScrollPos;
        private Vector2 _currentPlanScrollPos;
        private Vector2 _previousPlanScrollPos;

        private GoapGraph childGraph;
        public static System.Action PostGUI { get; set; }
        public bool refresh_neighbours = false;
        public bool _cleanNodes = false;

        public bool refreshNeighbours
        {
            set
            {
                refresh_neighbours = value;
            }
        }
        public bool cleanNodes
        {
            set
            {
                _cleanNodes = value;
            }
            get
            {
                return _cleanNodes;
            }
        }
        private float screenWidth
        { //for retina
            get { return EditorGUIUtility.currentViewWidth; }
        }
        private float screenHeight
        { //for retina
            get { return Screen.height; }
        }
        public GoapGraph currentChildGraph
        {
            get
            {
                return childGraph;
            }
            set
            {
                if (Application.isPlaying && value != null)
                {
                    return;
                }
                if (value != null)
                    value.childGraph = null;
                childGraph = value;
            }
        }
        public void ShowPrevieusPlan(Event guiEvent, Rect drawCanvas)
        {
            if (planner.noPlan)
                return;
            float totalHeight = 0;
            foreach (NodeGoal goal in allNodeGoals)
            {
                totalHeight += goal.height;
            }
            totalHeight = totalHeight < drawCanvas.height ? drawCanvas.height : totalHeight;


            Rect viewRect = new Rect(drawCanvas.x, drawCanvas.y, drawCanvas.width, drawCanvas.height - drawCanvas.y);
            Rect scrollRect = new Rect(drawCanvas.x, drawCanvas.y, _scrollSize, totalHeight);
            _previousPlanScrollPos = GUI.BeginScrollView(viewRect, _previousPlanScrollPos, scrollRect, false, true);

            Vector2 nodeStartPos = new Vector2(viewRect.x, viewRect.y);

            foreach (NodeBase node in lastPlan)
            {

                nodeStartPos = node.DrawNodeAsPreviousPlan(guiEvent, nodeStartPos, drawCanvas.width);

            }

            GUI.EndScrollView();
        }
        public void ShowCurrentPlan(Event guiEvent, Rect drawCanvas)
        {
            if (planner.noPlan)
                return;
            float totalHeight = 0;
            foreach (NodeGoal goal in allNodeGoals)
            {
                totalHeight += goal.height;
            }
            totalHeight = totalHeight < drawCanvas.height ? drawCanvas.height : totalHeight;


            Rect viewRect = new Rect(drawCanvas.x, drawCanvas.y, drawCanvas.width, drawCanvas.height - drawCanvas.y);
            Rect scrollRect = new Rect(drawCanvas.x, drawCanvas.y, _scrollSize, totalHeight);
            _currentPlanScrollPos = GUI.BeginScrollView(viewRect, _currentPlanScrollPos, scrollRect, false, true);

            Vector2 nodeStartPos = new Vector2(viewRect.x, viewRect.y);

            foreach (NodeAction action in currentPlan)
            {

                nodeStartPos = action.DrawNodeAsPlan(guiEvent, nodeStartPos, drawCanvas.width);

            }
            
            GUI.EndScrollView();
        }
        public void ShowGoalDesire(Event guiEvent, Rect drawCanvas)
        {

            float totalHeight = 0;
            foreach (NodeGoal goal in allNodeGoals)
            {
                totalHeight += goal.height;
            }
            totalHeight = totalHeight < drawCanvas.height ? drawCanvas.height : totalHeight;

            Rect viewRect = new Rect(drawCanvas.x, drawCanvas.y, drawCanvas.width, drawCanvas.height - drawCanvas.y);
            Rect scrollRect = new Rect(drawCanvas.x, drawCanvas.y, _scrollSize, totalHeight);
            _priorityGoalScrollPos = GUI.BeginScrollView(viewRect, _priorityGoalScrollPos, scrollRect, false, true);

            Vector2 nodeStartPos = new Vector2(viewRect.x, viewRect.y);

            foreach (NodeGoal goal in allNodeGoals)
            {
              
                    nodeStartPos = goal.DrawGoalDesireAsList(guiEvent, nodeStartPos, drawCanvas.width);   
                
            }

            GUI.EndScrollView();
        }

        public void ShowPlanView(Event guiEvent, Rect drawCanvas)
        {
            float totalWidth = 0;
            foreach (NodeBase node in currentPlan)
            {
                totalWidth += node.width;
            }
            totalWidth = totalWidth < drawCanvas.width ? drawCanvas.width : totalWidth;

            Rect viewRect = new Rect(drawCanvas.x, drawCanvas.y, drawCanvas.width, drawCanvas.height);
            Rect scrollRect = new Rect(0, 0, totalWidth, _scrollSize);

            _plannerScrollPos = GUI.BeginScrollView(viewRect, _plannerScrollPos, scrollRect, true, false);

            Vector2 nodeStartPos = new Vector2(viewRect.x, viewRect.y);

            foreach (NodeBase node in currentPlan)
            {

                if (!LastPlanNode(node))
                    nodeStartPos = DrawArrow();
            }
            GUI.EndScrollView();

        }
        Vector2 DrawArrow()
        {
            return Vector2.zero;
        }
        bool LastPlanNode(NodeBase node)
        {
            int lastItem = currentPlan.Count - 1;
            if (lastItem < 0)
                return false;
            return node == currentPlan[lastItem];
        }
        public void ShowActionNodesOnList(Event guiEvent, Rect drawCanvas, bool drawAll)
        {
            float totalHeight = 0;
            foreach (NodeAction action in allNodeActions)
            {
                totalHeight += action.height;
            }
            totalHeight = totalHeight < drawCanvas.height ? drawCanvas.height : totalHeight;
            Rect viewRect = new Rect(drawCanvas.x, drawCanvas.y, drawCanvas.width, drawCanvas.height - drawCanvas.y);
            Rect scrollRect = new Rect(0, 0, _scrollSize, totalHeight);
            _actionScrollPos = GUI.BeginScrollView(viewRect, _actionScrollPos, scrollRect, false, true);

            Vector2 nodeStartPos = new Vector2(viewRect.x, viewRect.y);

            foreach (NodeAction action in allNodeActions)
            {
                if (drawAll)
                {
                    nodeStartPos = action.DrawNodeAsList(guiEvent, nodeStartPos, drawCanvas.width - 20.0f);

                }
                else
                {
                    if (nodeStartPos.y > drawCanvas.yMax)
                        nodeStartPos = action.DrawNodeAsList(guiEvent, nodeStartPos, drawCanvas.width - 20.0f);

                }
            }

            GUI.EndScrollView();
        }

        public void ShowGoalNodesOnList(Event guiEvent, Rect drawCanvas, bool drawAll)
        {
            float totalHeight = 0;
            foreach (NodeGoal goal in allNodeGoals)
            {
                totalHeight += goal.height;
            }
            totalHeight = totalHeight < drawCanvas.height ? drawCanvas.height : totalHeight;

            totalHeight = totalHeight < drawCanvas.height ? drawCanvas.height : totalHeight;

            Rect viewRect = new Rect(drawCanvas.x, drawCanvas.y, drawCanvas.width, drawCanvas.height - drawCanvas.y);
            Rect scrollRect = new Rect(drawCanvas.x, drawCanvas.y, _scrollSize, totalHeight);
            _goalScrollPos = GUI.BeginScrollView(viewRect, _goalScrollPos, scrollRect, false, true);

            Vector2 nodeStartPos = new Vector2(viewRect.x, viewRect.y);

            foreach (NodeGoal goal in allNodeGoals)
            {
                if (drawAll)
                {
                    nodeStartPos = goal.DrawNodeAsList(guiEvent, nodeStartPos, drawCanvas.width - 20.0f);

                }
                else
                {
                    if (nodeStartPos.y > drawCanvas.yMax)
                        nodeStartPos = goal.DrawNodeAsList(guiEvent, nodeStartPos, drawCanvas.width - 20.0f);

                }
            }

            GUI.EndScrollView();

        }


        public void ShowNodesPlanning(Rect drawCanvas, bool draw_all)
        {
            for (int i = 0; i < allNodeGoals.Count; i++)
            {
                if (draw_all)
                {

                }
            }
        }
        public void ShowNodesPreview(Rect drawCanvas, bool draw_all)
        {
            for (int i = 0; i < allNodeGoals.Count; i++)
            {
                if (draw_all)
                {

                }
            }
        }
        public void ShowGraphControl(Event guiEvent)
        {

            ShowMenuBar(guiEvent);
            ShowWorldState(guiEvent);
            if (cleanNodes)
            {
                for (int i = allNodeActions.Count - 1; i >= 0; --i)
                {
                    if(allNodeActions[i].toDelete)
                    {
                        allNodeActions.RemoveAt(i);
                    }
                }
                for (int i = allNodeGoals.Count - 1; i >= 0; --i)
                {
                    if (allNodeGoals[i].toDelete)
                    {
                        allNodeGoals.RemoveAt(i);
                    }
                }
            }
            if (refresh_neighbours)
            {
                refresh_neighbours = false;
                ResetNeighbourds();
                UpdateNeighbourds();
            }
            if (PostGUI != null)
            {
                PostGUI();
                PostGUI = null;
            }
          
        }

        void ShowMenuBar(Event guiEvent)
        {
            GUILayout.BeginHorizontal();
            GoapPreferences.showNode = (GoapPreferences.ShowMode)GUILayout.SelectionGrid((int)GoapPreferences.showNode, selStrings, 2, EditorStyles.toolbarButton, GUILayout.Width(300));




            GUILayout.EndHorizontal();
        
        }
        void ShowWorldState(Event guiEvent)
        {
            if (worldState == null)
            {
                _worldStateRect.height = 0;
                return;
            }
            _worldStateRect.x = screenWidth - _worldStateSpacing - _worldStateBaseWidth;
            _worldStateRect.y = _worldStateSpacing;
            _worldStateRect.width = _worldStateBaseWidth;

            Rect worldStateButton = new Rect(_worldStateRect.x, _worldStateRect.y, _worldStateBaseWidth, _worldStateBaseHeight);
            EditorGUIUtility.AddCursorRect(worldStateButton, MouseCursor.Link);
            if (GUI.Button(worldStateButton, ""))
            {
                GoapPreferences.showWorldState = !GoapPreferences.showWorldState;
            }
            GUI.Box(worldStateButton, "", "windowShadow");

            if (GoapPreferences.showWorldState)
            {
                var lastSkin = GUI.skin;
                Rect viewRect = new Rect(_worldStateRect.x, _worldStateRect.y, _worldStateRect.width + 16, screenHeight - _worldStateRect.y - 30);
                Rect scrollRect = new Rect(_worldStateRect.x - 3, _worldStateRect.y, _worldStateRect.width, _worldStateRect.height);
                _worldStateScrollPos = GUI.BeginScrollView(viewRect, _worldStateScrollPos, scrollRect);
                GUILayout.BeginArea(_worldStateRect, title, "editorPanelNode");
                GUILayout.Space(5);
                GUI.skin = null;
                WorldStateEditor.ShowVariables(worldState, worldState as Object);
                GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(_worldStateRect.width - 10));
                GUILayout.Space(5);
                GUI.skin = lastSkin;

                if (guiEvent.type == EventType.Repaint)
                {
                    _worldStateRect.height = GUILayoutUtility.GetLastRect().yMax;
                }
                GUILayout.EndArea();
                GUI.EndScrollView();

                if (GUI.changed)
                    EditorUtility.SetDirty(this);
            }
            else
            {
                GUI.Box(_worldStateRect, title, "editorPanelNode");
                _worldStateRect.height = _worldStateBaseHeight;
            }
        }
    }
}
#endif
