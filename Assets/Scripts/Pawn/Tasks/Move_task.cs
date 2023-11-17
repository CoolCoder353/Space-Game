using System;
using UnityEngine;
using Tasks;

public class MoveTask : Task
{
    public MoveTask(int priority, TimeSpan timeout) : base(priority, timeout)
    {
    }

    public Vector3 Destination { get; set; }

    protected override void Perform(Pawn pawn)
    {
        // Code to move the pawn to the destination...
        Debug.Log($"{pawn.Name} is moving to {Destination}");
    }
}