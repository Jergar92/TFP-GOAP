#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace GOAP.Framework
{
    public class GraphEditor : EditorWindow
    {
        private static GoapGraph _currentGoapGraph = null;
        private GoapGraph _rootGraph = null;
        private int _rootGraphID = -1;

        private GoapOwner _goapOwner = null;
        private int _goapOwnerID = -1;

        private Event _guiEvent;
        //Canvas Rect
        private Rect _canvasTotalRect;
        private Rect _canvasActionTotalRect;
        private Rect _canvasActionRect;
        private Rect _canvasGoalTotalRect;
        private Rect _canvasGoalRect;
        private Rect _canvasRect;
        //Add Button Rect
        private Rect _addButtonRect;
        private Rect _addButtonTotal;


        //View Rect
        private Rect _viewRect;
        //UtilityRect
        private GUISkin _guiSkin;
        private readonly float _topMargin = 20.0f;
        private readonly float _bottomMargin = 5.0f;

        private readonly float _addButtonMargin = 60.0f;
        private readonly float _verticalMargin = 5.0f;

        private readonly float _verticalWSMargin = 350.0f;

        private readonly float size_ratio = 0.5f;
        private readonly float goalDesireSizeRatio = 0.2f;
        private readonly float currentPlanSize = 0.4f;
        private readonly float previeusPlanSize = 0.4f;

        private bool needRepaint = false;
        private bool needDirty = false;





        private GoapGraph rootGoapOwner
        {
            get
            {
                if (_rootGraph == null)
                {
                    _rootGraph = EditorUtility.InstanceIDToObject(_rootGraphID) as GoapGraph;
                }
                return _rootGraph;
            }
            set
            {
                _rootGraph = value;
                _rootGraphID = value != null ? value.GetInstanceID() : -1;
            }
        }
        private GoapOwner goapOwner
        {
            get
            {
                if (_goapOwner == null)
                {
                    _goapOwner = EditorUtility.InstanceIDToObject(_goapOwnerID) as GoapOwner;
                }
                return _goapOwner;
            }
            set
            {
                _goapOwner = value;
                _goapOwnerID = value != null ? value.GetInstanceID() : -1;
            }
        }


        private void OnEnable()
        {
            // guiSkin = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;
            _guiSkin = (GUISkin)Resources.Load("GoapSkin");

            //Editor Property
            minSize = new Vector2(700, 300);


            Repaint();

        }
        void OnCurrentGraphChanged()
        {
            UpdateReferences();
            needRepaint = true;
        }
        private void UpdateReferences()
        {
            rootGoapOwner = goapOwner != null ? goapOwner.goapGraph : rootGoapOwner;
            if (rootGoapOwner != null)
            {
                if (goapOwner != null)
                {
                    rootGoapOwner.agent = goapOwner;
                    rootGoapOwner.worldState = goapOwner.worldState;
                }
                rootGoapOwner.UpdateReferences();

                var current = GetCurrentGraph(rootGoapOwner);
                if (goapOwner != null)
                {
                    current.agent = goapOwner;
                    current.worldState = goapOwner.worldState;
                }
                current.UpdateReferences();
            }
        }
        public static GraphEditor OpenGraph(GoapOwner owner)
        {
            GraphEditor window = OpenGraph(owner.goapGraph, owner, owner.worldState);
            window.goapOwner = owner;
            return window;
        }
        public static GraphEditor OpenGraph(GoapGraph graph)
        {
            return OpenGraph(graph, graph.agent, graph.worldState);
        }
        public static GraphEditor OpenGraph(GoapGraph owner, Component agent, IWorldState world_state)
        {
            GraphEditor window = GetWindow<GraphEditor>();
            window._rootGraph = owner;
            window.goapOwner = null;
            if (window._rootGraph != null)
            {
                window._rootGraph.agent = agent;
                window._rootGraph.worldState = world_state;
                window._rootGraph.currentChildGraph = null;
                window._rootGraph.UpdateReferences();
            }

            return window;

        }

        void OnSelectionChange()
        {
            if (Selection.activeObject is GoapOwner)
            {
                EditorWindow last_window = focusedWindow;
                OpenGraph((GoapOwner)Selection.activeObject);
                if (last_window)
                    last_window.Focus();
                return;
            }

            if (Selection.activeObject is GoapGraph)
            {
                EditorWindow last_window = focusedWindow;
                OpenGraph((GoapGraph)Selection.activeObject);
                if (last_window)
                    last_window.Focus();
                return;
            }

            if (Selection.activeGameObject != null)
            {
                GoapOwner graph_owner = Selection.activeGameObject.GetComponent<GoapOwner>();
                if (graph_owner != null && graph_owner.goapGraph != null)
                {
                    EditorWindow last_window = focusedWindow;
                    OpenGraph(graph_owner);
                    if (last_window)
                        last_window.Focus();
                }

            }
        }
        void OnInspectorUpdate()
        {
            Repaint();
        }
        private void OnGUI()
        {
            _guiEvent = Event.current;
            GUI.skin.label.richText = true;
            GUI.skin = _guiSkin;
            if (goapOwner != null)
                _rootGraph = goapOwner.goapGraph;

            if (_rootGraph == null)
            {
                Debug.Log("ERROR NO GRAPH");
                return;
            }
            bool setDirty = false;
            if((_guiEvent.type==EventType.MouseUp&& _guiEvent.button!=2)||(_guiEvent.type==EventType.KeyUp&&(_guiEvent.keyCode==KeyCode.Return||GUIUtility.keyboardControl==0)))
            { 
                setDirty = true;
            }
            var _currentGraph = GetCurrentGraph(_rootGraph);
            if (!ReferenceEquals(_currentGraph, _currentGoapGraph))
            {
                _currentGoapGraph = _currentGraph;
                OnCurrentGraphChanged();
            }
            if (_currentGoapGraph == null || ReferenceEquals(_currentGoapGraph, null))
                return;

            DrawView(_guiEvent);

            if(needRepaint||rootGoapOwner.isRunning)
            {
                Repaint();
            }

            if(GUI.changed)
            {
                Repaint();

            }
            if(setDirty)
            {
                EditorUtility.SetDirty(_currentGraph);
            }
            GUI.skin = null;
        }
        GoapGraph GetCurrentGraph(GoapGraph root)
        {
            if (root.currentChildGraph == null)
                return root;
            return GetCurrentGraph(root.currentChildGraph);
        }
    
        void DrawView(Event gui_event)
        {

            switch (GoapPreferences.showNode)
            {
                case GoapPreferences.ShowMode.EDITOR_WINDOW:

                    DrawActionPannel(gui_event);
                    DrawGoalPanel(gui_event);
                    break;
                case GoapPreferences.ShowMode.PLANNING_VIEW:
                    DrawPlan(gui_event, false);
                    break;
                case GoapPreferences.ShowMode.PLANNING_PREVIEW:
                    DrawPlan(gui_event, true);
                    break;
                default:
                    break;
            }

            _currentGoapGraph.ShowGraphControl(gui_event);

        }



        void DrawActionPannel(Event _guiEvent)
        {

            _canvasActionRect = new Rect(_verticalMargin, _topMargin, (position.width - _verticalWSMargin) * size_ratio, position.height - _addButtonMargin);
            GUI.Box(_canvasActionRect, "Actions", "canvasBG");
            GUI.BeginGroup(_canvasActionRect);

            _canvasActionTotalRect = _canvasActionRect;
            _canvasActionTotalRect.x = 0;
            _canvasActionTotalRect.y = 0;

            GUI.BeginGroup(_canvasActionTotalRect);

            _viewRect = _canvasActionTotalRect;



            BeginWindows();
            _currentGoapGraph.ShowActionNodesOnList(_guiEvent, _viewRect, true);
            EndWindows();

            GUI.EndGroup();

            GUI.EndGroup();

            _addButtonRect = new Rect(_verticalMargin, _topMargin + position.height - _addButtonMargin, (position.width - _verticalWSMargin) * size_ratio, _addButtonMargin - _topMargin - _bottomMargin);
            GUI.Box(_addButtonRect, "");
            GUI.BeginGroup(_addButtonRect);
            _addButtonTotal = _addButtonRect;
            _addButtonTotal.x = 0;
            _addButtonTotal.y = 0;

            if (!Application.isPlaying && GUI.Button(_addButtonTotal, "Add Action", "Button"))
            {
                NodeAction action = _currentGoapGraph.AddNodeAction();
                GUI.EndGroup();
                return;
            }
            GUI.EndGroup();

        }
        void DrawGoalPanel(Event _guiEvent)
        {
            _canvasGoalRect = new Rect(_verticalMargin + (position.width - _verticalWSMargin) * size_ratio, _topMargin, ((position.width - _verticalWSMargin) * (1.0f - size_ratio)) - 10.0f, position.height - _addButtonMargin);
            GUI.Box(_canvasGoalRect, "Goals", "canvasBG");

            GUI.BeginGroup(_canvasGoalRect);

            _canvasGoalTotalRect = _canvasGoalRect;
            _canvasGoalTotalRect.x = 0;
            _canvasGoalTotalRect.y = 0;

            GUI.BeginGroup(_canvasGoalTotalRect);

            _viewRect = _canvasGoalTotalRect;





            BeginWindows();
            _currentGoapGraph.ShowGoalNodesOnList(_guiEvent, _viewRect, true);
            EndWindows();


            GUI.EndGroup();

            GUI.EndGroup();
            _addButtonRect = new Rect(_verticalMargin + (position.width - _verticalWSMargin) * size_ratio, _topMargin + position.height - _addButtonMargin, ((position.width - _verticalWSMargin) * (1.0f - size_ratio)) - 10.0f, _addButtonMargin - _topMargin - _bottomMargin);
            GUI.Box(_addButtonRect, "");
            GUI.BeginGroup(_addButtonRect);
            _addButtonTotal = _addButtonRect;
            _addButtonTotal.x = 0;
            _addButtonTotal.y = 0;

            if (!Application.isPlaying && GUI.Button(_addButtonTotal, "Add Goal", "Button"))
            {
                _currentGoapGraph.AddNodeGoal();
                GUI.EndGroup();
                return;
            }
            GUI.EndGroup();

        }

        void DrawPlan(Event _guiEvent, bool preview)
        {
            DrawGoalDesire(_guiEvent);
            DrawPlan(_guiEvent);
            DrawLastPlan(_guiEvent);
        }

        void DrawGoalDesire(Event _guiEvent)
        {

            Rect _goalDesire = new Rect(_verticalMargin, _topMargin, (position.width - _verticalWSMargin) * goalDesireSizeRatio, position.height- _topMargin);

            GUI.Box(_goalDesire, "Goal Desire", "canvasBG");
            GUI.BeginGroup(_goalDesire);
            _canvasTotalRect = _goalDesire;
            _canvasTotalRect.x = 0;
            _canvasTotalRect.y = 0;

            GUI.BeginGroup(_canvasTotalRect);

            _viewRect = _canvasTotalRect;
            _viewRect.x = 0;
            _viewRect.y = 0;




            BeginWindows();

            _currentGoapGraph.ShowGoalDesire(_guiEvent, _viewRect);


            EndWindows();

            GUI.EndGroup();

            GUI.EndGroup();
        }
        void DrawPlan(Event _guiEvent)
        {
            Rect planRect = new Rect(_verticalMargin + (position.width - _verticalWSMargin) * goalDesireSizeRatio, _topMargin, ((position.width - _verticalWSMargin) * currentPlanSize) - 10.0f, position.height -_topMargin);

            GUI.Box(planRect, "Current Plan", "canvasBG");
            GUI.BeginGroup(planRect);
            _canvasTotalRect = planRect;
            _canvasTotalRect.x = 0;
            _canvasTotalRect.y = 0;

            GUI.BeginGroup(_canvasTotalRect);

            _viewRect = _canvasTotalRect;
            _viewRect.x = 0;
            _viewRect.y = 0;




            BeginWindows();

            _currentGoapGraph.ShowCurrentPlan(_guiEvent, _viewRect);


            EndWindows();

            GUI.EndGroup();

            GUI.EndGroup();
        }
        void DrawLastPlan(Event _guiEvent)
        {
            Rect planRect = new Rect(_verticalMargin + (position.width - _verticalWSMargin) * (goalDesireSizeRatio+currentPlanSize), _topMargin, ((position.width - _verticalWSMargin) * previeusPlanSize) - 10.0f, position.height - _topMargin);

            GUI.Box(planRect, "Previous Plan", "canvasBG");
            GUI.BeginGroup(planRect);
            _canvasTotalRect = planRect;
            _canvasTotalRect.x = 0;
            _canvasTotalRect.y = 0;

            GUI.BeginGroup(_canvasTotalRect);

            _viewRect = _canvasTotalRect;
            _viewRect.x = 0;
            _viewRect.y = 0;




            BeginWindows();

            _currentGoapGraph.ShowPrevieusPlan(_guiEvent, _viewRect);


            EndWindows();

            GUI.EndGroup();

            GUI.EndGroup();
        }


    }
}
#endif