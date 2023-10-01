using System;
using System.Collections.Generic;
using UnityEngine;

public class Action_Callbacks_Two : MonoBehaviour
{

    public void FunctionC(Action action)
    {
        Debug.Log("Function C");
        action?.Invoke();
    }


}