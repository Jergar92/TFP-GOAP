using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GOAP.Framework
{
    public interface IFunctionAssignable
    {
        FunctionBase myFunction { get; set; }
    }
    public interface IFunctionAssignable<T> : IFunctionAssignable where T : FunctionBase { }
}