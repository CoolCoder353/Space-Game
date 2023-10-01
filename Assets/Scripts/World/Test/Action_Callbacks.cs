using System;
using System.Collections.Generic;
using UnityEngine;

public class Action_Callbacks : MonoBehaviour
{
    public Action_Callbacks_Two callbacks_Two;
    private void Start()
    {
        FunctionA(FunctionB);
    }
    public void FunctionA(Action functionB)
    {
        Debug.Log("Function A");
        functionB?.Invoke();

    }

    public void FunctionB()
    {
        Debug.Log("Function B");
        callbacks_Two.FunctionC(FunctionD);
    }
    public void FunctionD()
    {
        Debug.Log("Function D");
    }
}