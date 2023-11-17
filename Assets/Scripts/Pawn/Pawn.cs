using System;
using UnityEngine;
using System.Collections.Generic;
using Tasks;

public class Pawn : MonoBehaviour
{
    public string Name { get; set; }
    public float Health { get; set; }
    public float Hunger { get; set; }
    public float Mood { get; set; }
    //Tasks that only are added to the list when they are needed, e.g. if a pawn is hungry it will be added to the list
    //Note: This should NOT be filled with needs that are currently within acceptable bounds, e.g. if a pawn is not hungry it should not be added to the list
    public PriorityQueue<Task> VitalityTasks { get; set; }

    public PriorityQueue<Task> WorkTasks { get; set; }

    public Task currentTask { get; set; }

    private void Update()
    {
        if (VitalityTasks.Count > 0)
        {
            currentTask = VitalityTasks.Dequeue();
            PerformTask(currentTask);
        }
        else if (WorkTasks.Count > 0)
        {
            currentTask = WorkTasks.Dequeue();
            PerformTask(currentTask);
        }
    }

    public void PerformTask(Task task)
    {
        task.Execute(this);
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Eat(float amount)
    {
        Hunger -= amount;
        if (Hunger <= 50 && !VitalityTasks.Contains<EatTask>())
        {

            VitalityTasks.Enqueue(new EatTask(100 - Mathf.FloorToInt(Hunger), TimeSpan.FromDays(99)));
        }
    }

    public void Die()
    {
        // Code to handle death...
    }
}


