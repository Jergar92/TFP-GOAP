
using UnityEditor;
using UnityEngine;
namespace GOAP.Helper
{
    public static class UndoHelper
    {
        public static void CheckUndo(Object target, string name)
        {
            if (Application.isPlaying || target == null)
                return;

            Event e = Event.current;
            

            if (( (e.type == EventType.MouseDown || e.type == EventType.MouseUp) && e.button == 0) || (e.type == EventType.KeyUp))
            {
                Debug.Log("GOAP Undo");
                Debug.Log(target);
                Undo.RecordObject(target, name);
            }
        }
        public static void CheckDirty(Object target)
        {
            if (Application.isPlaying || target == null)
                return;

            if (GUI.changed)
            {
                Debug.Log("GOAP Dirty");
                Debug.Log(target);
                EditorUtility.SetDirty(target);
            }
        }
    }
}
