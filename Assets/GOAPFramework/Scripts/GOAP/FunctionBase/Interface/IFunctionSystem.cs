using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.Framework
{
    public interface IFunctionSystem
    {
        Component agent { get; }
        IWorldState worldState { get; }
        Object contextObj { get; }
    }
}
