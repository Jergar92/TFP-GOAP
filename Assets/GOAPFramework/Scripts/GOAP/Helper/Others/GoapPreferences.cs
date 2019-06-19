#if UNITY_EDITOR

using UnityEditor;

public static class GoapPreferences
{

    [System.Serializable]
    class GPSerializedData
    {
        public ShowMode _showMode = ShowMode.EDITOR_WINDOW;
        public bool _showModeState = true;

    }

    private static GPSerializedData data;
    private static GPSerializedData myData
    {
        get
        {
            if (data == null)
            {
                data = new GPSerializedData();
            }
            return data;
        }
    }

    public enum ShowMode
    {
        EDITOR_WINDOW,
        PLANNING_VIEW,
        PLANNING_PREVIEW
    }
    
    public static ShowMode showNode
    {
        get { return myData._showMode; }
        set { if (myData._showMode != value)
            {
                myData._showMode = value;
                Save();
            } }
    }
    public static bool showWorldState
    {
        get { return myData._showModeState; }
        set
        {
            if (myData._showModeState != value)
            {
                myData._showModeState = value;
                Save();
            }
        }
    }
    static void Save()
    {
        //EditorPrefs.SetString("NodeCanvas.EditorPreferences", JSONSerializer.Serialize(typeof(SerializedData), data));
    }
}
#endif