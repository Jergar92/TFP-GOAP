#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using GOAP.Framework;
using GOAP.Helper;
namespace GOAP.Editor
{
    [CustomEditor(typeof(GoapOwner), true)]
    public class GoapOwnerInspector : UnityEditor.Editor
    {
        private GoapOwner goapOwner
        {
            get { return target as GoapOwner; }
        }

        public override void OnInspectorGUI()
        {
            UndoHelper.CheckUndo(goapOwner, "GoapOwner Inspector");


            if (goapOwner.goapGraph == null)
            {

                if (!Application.isPlaying && GUILayout.Button("Create NEW Goap"))
                {
                    GoapGraph new_goap = CreateAsset();

                    if (new_goap != null)
                    {
                        GraphEditor.OpenGraph(goapOwner);

                    }

                    goapOwner.goapGraph = (GoapGraph)EditorGUILayout.ObjectField("Goal Graph", goapOwner.goapGraph, typeof(GoapGraph), false);
                    return;
                }
            }

            //TODO Color class
            if (goapOwner.goapGraph != null)
            {
                if (GUILayout.Button(("Edit Goap")))
                {
                    GraphEditor.OpenGraph(goapOwner);
                }
            }
            if (!Application.isPlaying)
            {
                goapOwner.goapGraph = (GoapGraph)EditorGUILayout.ObjectField("Goal Graph", goapOwner.goapGraph, typeof(GoapGraph), true);
            }

            UndoHelper.CheckDirty(goapOwner);

            if(GUI.changed)
            {
                EditorUtility.SetDirty(goapOwner);
                if(goapOwner.goapGraph!=null)
                {
                    EditorUtility.SetDirty(goapOwner.goapGraph);
                }
            }
        }
        private GoapGraph CreateAsset()
        {

            string path = EditorUtility.SaveFilePanelInProject("Create a GOAP Graph", "NewGoapGraph.asset", "asset", "");
            if (string.IsNullOrEmpty(path))
                return null;
            ScriptableObject asset = CreateInstance(typeof(GoapGraph));
            AssetDatabase.CreateAsset(asset, path);

            GoapGraph graph = (GoapGraph)asset;
            if (graph != null)
            {
                goapOwner.goapGraph = graph;
                EditorUtility.SetDirty(goapOwner);
                EditorUtility.SetDirty(graph);
                AssetDatabase.SaveAssets();
            }


            return graph;

        }

    }
}
#endif