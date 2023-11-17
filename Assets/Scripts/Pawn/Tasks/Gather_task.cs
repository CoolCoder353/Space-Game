using System;
using UnityEngine;
using Tasks;

public class GatherResourceTask : Task
{
    public GatherResourceTask(int priority, TimeSpan timeout) : base(priority, timeout)
    {
    }

    public string Resource { get; set; }

    protected override void Perform(Pawn pawn)
    {
        // Code to gather the resource...
        Debug.Log($"{pawn.Name} is gathering {Resource}");
    }
}