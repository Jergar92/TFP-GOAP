#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GOAP.Helper;

namespace GOAP.Framework
{
    partial class FunctionBase
    {

        private bool _isUnfolded = true;

        public static FunctionBase copyFunction { get; set; }
        private bool isUnfolded
        {
            get
            {
                return _isUnfolded;
            }
            set
            {
                _isUnfolded = value;
            }
        }

        public static void ShowFunctionInspector(FunctionBase function, Action<FunctionBase> callback)
        {
            EditorHelper.Separator();

            if (function.ownerSystem == null)
            {
                GUILayout.Label("ERROR: Owner System is null");
                return;
            }
            if (function.agentIsOverride && function.override_agent == null)
                function.override_agent = new FunctionAgent();
            if (ShowTaskTitlebar(function, callback))
            {

                ShowAgentField(function);
                function.OnTaskInspector();
            }
            EditorHelper.Separator();


        }
        static void ShowAgentField(FunctionBase function)
        {
            if (function.agentType == null)
            {
                GUILayout.Label("ERROR: AgentType is null");
                return;
            }
            if (function.agentIsOverride)
            {

            }
            else
            {

            }
        }
        static bool ShowTaskTitlebar(FunctionBase function, Action<FunctionBase> callback)
        {
            GUILayout.BeginHorizontal("box");

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                if (callback != null)
                    callback(null);
                return false;
            }

            GUILayout.Label("<b>" + (function.isUnfolded ? "▼ " : "► ") + function.name + "</b>");
            GUILayout.EndHorizontal();
            Rect title_rect = GUILayoutUtility.GetLastRect();
            EditorGUIUtility.AddCursorRect(title_rect, MouseCursor.Link);

            var e = Event.current;
            if (e.button == 0 && e.type == EventType.MouseDown && title_rect.Contains(e.mousePosition))
                e.Use();
            if (e.button == 0 && e.type == EventType.MouseUp && title_rect.Contains(e.mousePosition))
            {
                function.isUnfolded = !function.isUnfolded;
                e.Use();
            }
            return function.isUnfolded;
        }
        virtual protected void OnTaskInspector()
        {

            DrawDefaultInspector();
        }
        protected void DrawDefaultInspector()
        {
            EditorHelper.ShowAutomaticEditor(this);
        }

    }
}
#endif