using System;
using System.Collections.Generic;
using GOAP.Helper;
namespace FullSerializer
{

    public class fsUnityObjectConverter : fsConverter
    {

        public override bool CanProcess(Type type)
        {
            return typeof(UnityEngine.Object).RHelperIsAssignableFrom(type);
        }
        public override bool RequestCycleSupport(Type storageType)
        {
            return false;
        }
        public override bool RequestInheritanceSupport(Type storageType)
        {
            return false;
        }
        public override object CreateInstance(fsData data, Type storageType)
        { 
       	return null;
		}

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            List<UnityEngine.Object> database = Serializer.Context.Get<List<UnityEngine.Object>>();
            UnityEngine.Object o = instance as UnityEngine.Object;

            int index = -1;
            for (int i = 0; i < database.Count; i++)
            {
                if(ReferenceEquals(database[i],o))
                {
                    index = i;
                    break;
                }
            }

            if (database.Count == 0)
                database.Add(null);

            if(index<=0)
            {
                index = database.Count;
                database.Add(o);
            }
            serialized = new fsData(index);
            return fsResult.Success;
        }
        public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
        {
            List<UnityEngine.Object> database = Serializer.Context.Get<List<UnityEngine.Object>>();
            int index =(int) data.AsInt64;

            if (index >= database.Count)
                return fsResult.Warn("Error unity object has not been serialized");

            instance = database[index];
            return fsResult.Success;
        }
    }
}
