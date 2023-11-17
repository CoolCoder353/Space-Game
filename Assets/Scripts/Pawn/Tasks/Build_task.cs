using System;
using UnityEngine;
using Tasks;
public class BuildTask : Task
{
    public BuildTask(int priority, TimeSpan timeout) : base(priority, timeout)
    {
    }

    public string Structure { get; set; }

    protected override void Perform(Pawn pawn)
    {
        // Code to build the structure...
        Debug.Log($"{pawn.Name} is building {Structure}");
    }
}