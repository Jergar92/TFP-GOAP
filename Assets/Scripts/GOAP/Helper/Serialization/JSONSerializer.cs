using System;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;

// lock info
// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement

namespace GOAP.Serialization
{
    public static class JSONSerializer 
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoad]
        class StartUp
        {
            static StartUp()
            {
                appIsPlaying = false;
                UnityEditor.EditorApplication.playModeStateChanged += AppIsPlaying;
            }
        }

        private static void AppIsPlaying(UnityEditor.PlayModeStateChange state)
        {
            appIsPlaying = Application.isPlaying;
            Debug.Log(state);
        }
#endif
        private static Dictionary<string, fsData> cache = new Dictionary<string, fsData>();
        private static object serializerLock = new object();
        private static fsSerializer serializer = new fsSerializer();
        private static bool init = false;

        public static bool appIsPlaying;

        public static string Serialize(Type type, object value, bool pretyJSON = false, List<UnityEngine.Object> objectRef = null)
        {
            lock(serializerLock)
            {
                if(!init)
                {
                    serializer.AddConverter(new fsUnityObjectConverter());
                    init = true;
                }

                if (objectRef != null)                
                    serializer.Context.Set<List<UnityEngine.Object>>(objectRef);
                

                fsData data;
                serializer.TrySerialize(type, value, out data).AssertSuccess();

                cache[fsJsonPrinter.CompressedJson(data)] = data;
                if (pretyJSON)
                    return fsJsonPrinter.PrettyJson(data);
                return fsJsonPrinter.CompressedJson(data);
            }
        }

        public static T Deserialize<T>(string serializedState, List<UnityEngine.Object> objectRef = null)
        {
            return (T)Deserialize(typeof(T), serializedState, objectRef);
        }
        public static object Deserialize(Type type, string serializedState, List<UnityEngine.Object> objectRef = null)
        {
            lock (serializerLock)
            {
                if (!init)
                {
                    serializer.AddConverter(new fsUnityObjectConverter());
                    init = true;
                }

                if (objectRef != null)
                    serializer.Context.Set<List<UnityEngine.Object>>(objectRef);

                fsData data = null;
                cache.TryGetValue(serializedState, out data);
                if(data==null)
                {

                    data = fsJsonParser.Parse(serializedState);
                    cache[serializedState] = data;
                }
                object ret = null;
                serializer.TryDeserialize(data, type, ref ret).AssertSuccess();

                return ret;

            }
        }
    }
}
