#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using UnityEngine;
using UnityEditor;
using GOAP.Helper;
namespace GOAP.Framework
{
    public partial class NodeBase
    {

        [SerializeField]
        private bool _showInfo = true;

        private Vector2 scrollPosition;

        private Rect _nodeRect = new Rect(0, 0, 0, 0);

        private bool _preIsUnfolded = true;
        private bool _effIsUndolded = true;
        private bool _functionIsUnfolded = true;
        private static readonly int checkBoxSize = 16;

        public float height
        {
            get
            {
                return _nodeRect.height;
            }
        }
        public float width
        {
            get
            {
                return _nodeRect.width;
            }
        }
        private bool functionIsUnfolded
        {
            get
            {
                return _functionIsUnfolded;
            }
            set
            {
                _functionIsUnfolded = value;
            }
        }
        private bool preIsUnfolded
        {
            get
            {
                return _preIsUnfolded;
            }
            set
            {
                _preIsUnfolded = value;
            }
        }
        private bool effIsUnfolded
        {
            get
            {
                return _effIsUndolded;
            }
            set
            {
                _effIsUndolded = value;
            }
        }
        

        readonly public static Vector2 node_min_size = new Vector2(100, 20);
        readonly private float node_title_space = 30.0f;
        readonly private float node_inspector_space = 10.0f;
        public Vector2 DrawNodeAsPreviousPlan(Event gui_event, Vector2 start_point, float width)
        {
            if (!Application.isPlaying)
                return start_point;
            Vector2 ret_end_point = start_point;


            ret_end_point.y += DrawNodeWindowAsPreviousPlan(gui_event, start_point, width);

            return ret_end_point;
        }
        float DrawNodeWindowAsPreviousPlan(Event gui_event, Vector2 start_point, float width)
        {
            _nodeRect.width = width;
            _nodeRect.x = start_point.x;
            _nodeRect.y = start_point.y;


            string title = name != null ? name : "Node";

            GUI.Box(_nodeRect, "");

            var last_skin = GUI.skin;
            GUILayout.BeginArea(_nodeRect, title, "editorPanel" + nodeName);
            GUILayout.Space(5);
            string text = "";
            if (status == Status.START)
            {
                GUI.color = Color.white;
                text = "START";
            }
            if (status == Status.RUNNING)
            {
                GUI.color = Color.cyan;
                text = "RUNNING";

            }
            if (status == Status.SUCCESS)
            {
                GUI.color = Color.green;
                text = "SUCCESS";

            }
            if (status == Status.FAILURE || status == Status.ERROR)
            {
                GUI.color = Color.red;
                text = "FAILURE";

            }
            GUILayout.Box(text, "textarea");

            GUI.color = Color.white;

            // GUILayout.Label(goal.goalPriority.ToString());
            GUI.skin = null;
            GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(_nodeRect.width - node_inspector_space));
            GUILayout.Space(5);
            GUI.skin = last_skin;
            if (gui_event.type == EventType.Repaint)
            {
                _nodeRect.height = GUILayoutUtility.GetLastRect().yMax;
            }
            GUILayout.EndArea();

            return _nodeRect.height;
        }

        public Vector2 DrawNodeAsPlan(Event gui_event, Vector2 start_point, float width)
        {
            if (!Application.isPlaying)
                return start_point;
            Vector2 ret_end_point = start_point;


            ret_end_point.y += DrawNodeWindowAsPlan(gui_event, start_point, width);

            return ret_end_point;
        }

        float DrawNodeWindowAsPlan(Event gui_event, Vector2 start_point, float width)
        {
            _nodeRect.width = width;
            _nodeRect.x = start_point.x;
            _nodeRect.y = start_point.y;


            string title = name != null ? name : "Node";

            GUI.Box(_nodeRect, "");

            var last_skin = GUI.skin;
            GUILayout.BeginArea(_nodeRect, title, "editorPanel" + nodeName);
            GUILayout.Space(5);
            string text = "";
            NodeAction action = this as NodeAction;
            if (action.myAction.status == Status.START )
            {
                GUI.color = Color.white;
                text = "START";
            }
            if (action.myAction.status == Status.RUNNING)
            {
                GUI.color = Color.cyan;
                text = "RUNNING";

            }
            if (action.myAction.status == Status.SUCCESS)
            {
                GUI.color = Color.green;
                text = "SUCCESS";

            }
            if (action.myAction.status == Status.FAILURE || action.myAction.status == Status.ERROR)
            {
                GUI.color = Color.red;
                text = "FAILURE";

            }
            GUILayout.Box(text, "textarea");

            GUI.color = Color.white;

            // GUILayout.Label(goal.goalPriority.ToString());
            GUI.skin = null;
            GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(_nodeRect.width - node_inspector_space));
            GUILayout.Space(5);
            GUI.skin = last_skin;
            if (gui_event.type == EventType.Repaint)
            {
                _nodeRect.height = GUILayoutUtility.GetLastRect().yMax;
            }
            GUILayout.EndArea();

            return _nodeRect.height;
        }

        public Vector2 DrawGoalDesireAsList(Event gui_event, Vector2 start_point, float width)
        {
            if (!GetType().Equals(typeof(NodeGoal)) ||  !Application.isPlaying)
                return start_point;
            Vector2 ret_end_point = start_point;


            ret_end_point.y += DrawGoalDesireWindowAsList(gui_event, start_point, width);

            return ret_end_point;
        }
        float DrawGoalDesireWindowAsList(Event gui_event, Vector2 start_point, float width)
        {
            _nodeRect.width = width;
            _nodeRect.x = start_point.x;
            _nodeRect.y = start_point.y;
      

            string title = name != null ? name : "Node";

            GUI.Box(_nodeRect, "");

            GUI.color = Color.white;
            var last_skin = GUI.skin;
            GUILayout.BeginArea(_nodeRect, title, "editorPanel" + nodeName);
            NodeGoal goal = this as NodeGoal;
            NodeGoal priorityGoal = goapGraph.priorityGoalRef;
            NodeGoal nodeGoal = goapGraph.currentGoal;

            if (priorityGoal != null && nodeGoal!=null)
            {
                if (nodeGoal.name == goal.name)
                {
                    GUI.color = Color.cyan;
                }
                if (priorityGoal.name == goal.name)
                {
                    GUI.color = Color.green;
                }

            }
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();               
            GUILayout.Box("Goal Priority: " + goal.goalPriority.ToString(), "textarea");
            GUILayout.EndHorizontal();

            // GUILayout.Label(goal.goalPriority.ToString());
            GUI.color = Color.white;
            GUI.skin = null;

            GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(_nodeRect.width - node_inspector_space));
            GUILayout.Space(5);
            GUI.skin = last_skin;
            if (gui_event.type == EventType.Repaint)
            {
                _nodeRect.height = GUILayoutUtility.GetLastRect().yMax;
            }
            GUILayout.EndArea();

            return _nodeRect.height;
        }
        public Vector2 DrawNodeAsList(Event gui_event, Vector2 start_point, float width)
        {
            Vector2 ret_end_point = start_point;


            ret_end_point.y += DrawNodeWindowAsList(gui_event, start_point, width);

            return ret_end_point;
        }
        float DrawNodeWindowAsList(Event gui_event, Vector2 start_point, float width)
        {
            _nodeRect.width = width;
            _nodeRect.x = start_point.x;
            _nodeRect.y = start_point.y;
            EditorGUIUtility.AddCursorRect(_nodeRect, MouseCursor.Link);
            if (GUI.Button(new Rect(_nodeRect.x, _nodeRect.y, _nodeRect.width-30, node_title_space), ""))
            {
                _showInfo = !_showInfo;
            }

            string title = name != null ? name : "Node";

            GUI.Box(_nodeRect, "");

            GUI.color = Color.white;
            if (_showInfo)
            {
                var last_skin = GUI.skin;
                GUILayout.BeginArea(_nodeRect, title, "editorPanel" + nodeName);
                if (!Application.isPlaying&&GUI.Button(new Rect(_nodeRect.x+ _nodeRect.width-25, 0,25, 25), "X"))
                {
                    DeletePopUp();
                }
                GUILayout.Space(5);
                GUI.skin = null;
                ShowNodeInspector(new Vector2(0, node_title_space), width);
                GUILayout.Box("", GUILayout.Height(5), GUILayout.Width(_nodeRect.width - node_inspector_space));
                GUILayout.Space(5);
                GUI.skin = last_skin;
                if (gui_event.type == EventType.Repaint)
                {
                    _nodeRect.height = GUILayoutUtility.GetLastRect().yMax;
                }
                GUILayout.EndArea();
            }
            else
            {
                GUI.Box(_nodeRect, title, "editorPanel" + nodeName);
                if (!Application.isPlaying&&GUI.Button(new Rect(_nodeRect.x + _nodeRect.width - 25, _nodeRect.y, 25, 25), "X"))
                {
                    DeletePopUp();
                }
                _nodeRect.height = node_title_space;

            }
            return _nodeRect.height;
        }
        void DeletePopUp()
        {

            if (EditorUtility.DisplayDialog("Delete Node '" + name + "'?", "Delete Node '" + name + "'? Yes, No", "Yes","No"))
            {
                _toDelete = true;
                goapGraph.cleanNodes = true;

                GUIUtility.keyboardControl = 0;
                GUIUtility.hotControl = 0;
            }

        }
        //This is the callback function of the GUILayout.window. Everything here is called INSIDE the node Window callback.
        void NodeWindowGUI(int ID)
        {
            /*
           var e = Event.current;

           ShowHeader();
           ShowPossibleErrors();
           HandleEvents(e);
           ShowStatusIcons();
           ShowBreakpointMark();
           ShowNodeContents();
           HandleContextMenu(e);
           HandleNodePosition(e);
           */
        }
        void ShowNodeInspector(Vector2 start_point, float width)
        {
            IFunctionAssignable assignable = (IFunctionAssignable)this;
            System.Type task_type = null;
            foreach (var iType in GetType().GetInterfaces())
            {
                if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IFunctionAssignable<>))
                {
                    task_type = iType.GetGenericArguments()[0];
                    break;
                }
            }

            if (task_type != null)
            {
                ActionGoalInspector();
                EditorHelper.FunctionInfo(assignable.myFunction, goapGraph, task_type, (t) =>
                {
                    assignable.myFunction = t;
                });
                EditorGUI.BeginChangeCheck();

                if (this is NodeGoal)
                    ShowDesireTab(assignable.myFunction);
                if (this is NodeAction)
                {
                    ShowPreconditionTab(assignable.myFunction);
                    ShowEffectTab(assignable.myFunction);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Debug.Log("Check");
                    goapGraph.refreshNeighbours = true;
                }
            }

        }
        void ActionGoalInspector()
        {
            if (ShowSpecialCase())
            {
                name = EditorGUILayout.TextField("Node Name", name);
                if (this is NodeAction)
                {
                    GUILayout.BeginHorizontal();
                    var nodeAction = (this as NodeAction);
                    GUI.enabled = !nodeAction.isActionCostVariable;
                    nodeAction.actionCost = EditorGUILayout.IntField("Action Cost", nodeAction.actionCost);
                    GUI.enabled = true;
                    if (!Application.isPlaying && GUILayout.Button("V", GUILayout.Width(checkBoxSize), GUILayout.Height(checkBoxSize)))
                    {
                        nodeAction.isActionCostVariable = !nodeAction.isActionCostVariable;
                    }
                    GUILayout.EndHorizontal();
                    nodeAction.failFrequency = EditorGUILayout.Slider("Fail Frequency", nodeAction.failFrequency, 0.0f, 1.0f);

                    nodeAction.interrumpible = EditorGUILayout.Toggle("Interrumpible", nodeAction.interrumpible);

                }
                if (this is NodeGoal)
                {
                    var nodeGoal = (this as NodeGoal);
                    nodeGoal.goalPriority = EditorGUILayout.IntField("Goal Desire", nodeGoal.goalPriority);

                }
            }
        }
        private bool ShowSpecialCase()
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("<b>" + (functionIsUnfolded ? "▼ " : "► ") + name + " Parameters </b>");
            GUILayout.EndHorizontal();
            Rect title_rect = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(title_rect, MouseCursor.Link);
            var e = Event.current;
            if (e.button == 0 && e.type == EventType.MouseDown && title_rect.Contains(e.mousePosition))
                e.Use();
            if (e.button == 0 && e.type == EventType.MouseUp && title_rect.Contains(e.mousePosition))
            {
                functionIsUnfolded = !functionIsUnfolded;
                e.Use();
            }
            return functionIsUnfolded;
        }
        void ShowPreconditionTab(FunctionBase function)
        {
            var action = this as NodeAction;
            if (action == null)
                return;
            if (ShowPreconditionTitlebar(function))
            {
            
                var e = Event.current;

                foreach (PEParameter item in action.preconditions)
                {
                    EditorHelper.ShowPEParameterFieldPrecondition(item,
                    (parameter) =>
                    {
                        action.RemovePrecondition(parameter);
                        goapGraph.refreshNeighbours = true;
                        return;
                    });

                }

                foreach (PEMethod item in action.functionPreconditions)
                {
                    EditorHelper.ShowPEMethodField(item,
                    (method) =>
                    {
                        action.RemovePrecondition(method);
                        goapGraph.refreshNeighbours = true;
                        return;
                    });

                }
                if (GUILayout.Button("Add Precondition"))
                {
                    Action<string> selected_var = (t) =>
                    {
                        action.AddPrecondition(worldState.GetVariable(t));
                        goapGraph.refreshNeighbours = true;

                    };
                    System.Action<string> selectedGOMethod = (f) =>
                    {
                        action.AddPrecondition(worldState.GetFunctionVariable(f));
                        goapGraph.refreshNeighbours = true;

                    };

       
                    GenericMenu menu = new GenericMenu();
                    GenericMenu.MenuFunction2 Selected = delegate (object obj)
                    {
                        selected_var(obj.ToString());
                    };
                    GenericMenu.MenuFunction2 SelectedFunc = delegate (object obj)
                    {
                        selectedGOMethod(obj.ToString());
                    };
                    List<string> var_names = new List<string>();
                    if (worldState != null)
                        var_names.AddRange(worldState.GetVariableNames());
                    foreach (string name in var_names)
                    {
                        menu.AddItem(new GUIContent(name), false, Selected, worldState.GetVariable(name));
                    }
                    List<string> func_names = new List<string>();
                    if (worldState != null)
                        func_names.AddRange(worldState.GetFunctionVariableNames());
                    foreach (string name in func_names)
                    {
                        menu.AddItem(new GUIContent(name), false, SelectedFunc, worldState.GetFunctionVariable(name));
                    }

                    menu.ShowAsContext();
                    e.Use();
                }

            }

        }
        private bool ShowPreconditionTitlebar(FunctionBase function)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("<b>" + (preIsUnfolded ? "▼ " : "► ") + "Preconditions " + (function == null ? "Function is NULL! </b>" : "</b>"));
            GUILayout.EndHorizontal();
            Rect title_rect = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(title_rect, MouseCursor.Link);
            var e = Event.current;
            if (e.button == 0 && e.type == EventType.MouseDown && title_rect.Contains(e.mousePosition))
                e.Use();
            if (e.button == 0 && e.type == EventType.MouseUp && title_rect.Contains(e.mousePosition))
            {
                preIsUnfolded = !preIsUnfolded;
                e.Use();
            }
            return preIsUnfolded && function != null;
        }
        public void ShowEffectTab(FunctionBase function)
        {
            var action = this as NodeAction;
            if (action == null)
                return;

            if (ShowEffectTitlebar(function))
            {
                
                var e = Event.current;
              
                foreach (PEParameter item in action.effects)
                {
                    EditorHelper.ShowPEParameterFieldEffect(item,
                    (parameter) =>
                    {
                        action.RemoveEffect(parameter);
                        goapGraph.refreshNeighbours = true;
                        return;
                    });

                }
                foreach (PEMethod item in action.functionEffects)
                {
                    EditorHelper.ShowPEMethodField(item,
                    (method) =>
                    {
                        action.RemoveEffect(method);
                        goapGraph.refreshNeighbours = true;
                        return;
                    });

                }
                if (GUILayout.Button("Add Effect"))
                {
                    Action<string> selected_var = (t) =>
                    {
                        action.AddEffect(worldState.GetVariable(t));
                        goapGraph.refreshNeighbours = true;

                    };

                    System.Action<string> selectedGOMethod = (f) =>
                    {
                        action.AddEffect(worldState.GetFunctionVariable(f));
                        goapGraph.refreshNeighbours = true;

                    };
               

                    GenericMenu menu = new GenericMenu();
                    GenericMenu.MenuFunction2 Selected = delegate (object obj)
                    {
                        selected_var(obj.ToString());
                    };
                    GenericMenu.MenuFunction2 SelectedFunc = delegate (object obj)
                    {
                        selectedGOMethod(obj.ToString());
                    };
                    List<string> var_names = new List<string>();
                    if (worldState != null)
                        var_names.AddRange(action.worldState.GetVariableNames());
                    foreach (string name in var_names)
                    {
                        menu.AddItem(new GUIContent(name), false, Selected, action.worldState.GetVariable(name));
                    }

                    List<string> func_names = new List<string>();
                    if (worldState != null)
                        func_names.AddRange(worldState.GetFunctionVariableNames());
                    foreach (string name in func_names)
                    {
                        menu.AddItem(new GUIContent(name), false, SelectedFunc, worldState.GetFunctionVariable(name));
                    }
                   
                    menu.ShowAsContext();
                    e.Use();
                }

            }

        }

        private bool ShowEffectTitlebar(FunctionBase function)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("<b>" + (effIsUnfolded ? "▼ " : "► ") + "Effect " + (function == null ? "Function is NULL! </b>" : "</b>"));
            GUILayout.EndHorizontal();
            Rect title_rect = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(title_rect, MouseCursor.Link);
            var e = Event.current;
            if (e.button == 0 && e.type == EventType.MouseDown && title_rect.Contains(e.mousePosition))
                e.Use();
            if (e.button == 0 && e.type == EventType.MouseUp && title_rect.Contains(e.mousePosition))
            {
                effIsUnfolded = !effIsUnfolded;
                e.Use();
            }
            return effIsUnfolded && function != null;
        }
        public void ShowDesireTab(FunctionBase function)
        {
            var goal = this as NodeGoal;
            if (goal == null)
                return;

            if (ShowDesireTitlebar(function))
            {    

                var e = Event.current;            

                foreach (PEParameter item in goal.desires)
                {
                    EditorHelper.ShowPEParameterFieldEffect(item,
                    (parameter) =>
                    {
                        goal.RemoveDesire(parameter);
                        goapGraph.refreshNeighbours = true;
                        return;
                    });

                }
                foreach (PEMethod item in goal.functionDesires)
                {
                    EditorHelper.ShowPEMethodField(item,
                    (method) =>
                    {
                        goal.RemoveDesire(method);
                        goapGraph.refreshNeighbours = true;
                        return;
                    });

                }
                if (GUILayout.Button("Add Desire"))
                {
                    Action<string> selected_var = (t) =>
                    {
                        goal.AddDesire(worldState.GetVariable(t));
                        goapGraph.refreshNeighbours = true;
                    };

                    System.Action<string> selectedGOMethod = (f) =>
                    {
                        goal.AddDesire(worldState.GetFunctionVariable(f));
                        goapGraph.refreshNeighbours = true;

                    };           



                    GenericMenu menu = new GenericMenu();
                    GenericMenu.MenuFunction2 Selected = delegate (object obj)
                    {
                        selected_var(obj.ToString());
                    };

                    GenericMenu.MenuFunction2 SelectedFunc = delegate (object obj)
                    {
                        selectedGOMethod(obj.ToString());
                    };

                    List<string> var_names = new List<string>();
                    if (worldState != null)
                        var_names.AddRange(goal.worldState.GetVariableNames());
                    foreach (string name in var_names)
                    {
                        menu.AddItem(new GUIContent(name), false, Selected, goal.worldState.GetVariable(name));
                    }

                    List<string> func_names = new List<string>();
                    if (worldState != null)
                        func_names.AddRange(worldState.GetFunctionVariableNames());
                    foreach (string name in func_names)
                    {
                        menu.AddItem(new GUIContent(name), false, SelectedFunc, worldState.GetFunctionVariable(name));
                    }
                    menu.ShowAsContext();
                    e.Use();
                }

            }

        }

        private bool ShowDesireTitlebar(FunctionBase function)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("<b>" + (effIsUnfolded ? "▼ " : "► ") + "Effect " + (function == null ? "Function is NULL! </b>" : "</b>"));
            GUILayout.EndHorizontal();
            Rect title_rect = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(title_rect, MouseCursor.Link);
            var e = Event.current;
            if (e.button == 0 && e.type == EventType.MouseDown && title_rect.Contains(e.mousePosition))
                e.Use();
            if (e.button == 0 && e.type == EventType.MouseUp && title_rect.Contains(e.mousePosition))
            {
                effIsUnfolded = !effIsUnfolded;
                e.Use();
            }
            return effIsUnfolded && function != null;
        }
        virtual protected void OnNodeInspectorGUI() { DrawDefaultInspector(); }
        protected void DrawDefaultInspector()
        {
            EditorHelper.ShowAutomaticEditor(this);
        }
    }
}
#endif