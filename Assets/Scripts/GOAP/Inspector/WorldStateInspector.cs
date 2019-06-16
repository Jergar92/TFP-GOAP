#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using GOAP.Framework;
namespace GOAP.Editor
{
    [CustomEditor(typeof(WorldState))]
    public class WorldStateInspector : UnityEditor.Editor
    {

        private WorldState worldState
        {
            get { return (WorldState)target; }
        }

        public override void OnInspectorGUI()
        {
            if (Event.current.isMouse)
            {
                Repaint();
            }
            WorldStateEditor.ShowVariables(worldState, worldState);

            if (Application.isPlaying)
            {
                Repaint();
            }
        }
    }
}
#endif