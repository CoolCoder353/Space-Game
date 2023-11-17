using System;
using Tasks;

public class EatTask : Task
{
    public EatTask(int priority, TimeSpan timeout) : base(priority, timeout) { }

    protected override void Perform(Pawn pawn)
    {
        // Define what happens when the task is performed
        ////pawn.Eat();

        // Update the progress of the task
        UpdateProgress(1);
    }
}