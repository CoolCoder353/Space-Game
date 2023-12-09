using System;
using UnityEngine;
using System.Collections.Generic;
using Tasks;
using WorldGeneration;
using System.Collections;

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

    private global_job_manager global_Job_Manager;

    private World current_world { get; set; }

    //TODO: Incorperate this into some sort of skills or attribute management system.
    public float speed = 1f;



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
        else
        {
            //Ask the global job manager if there are any jobs available that we are allowed to do
            //If there are, add them to the work tasks list
            //TODO: Make this be selected via the priorities of the pawn.

            global_Job_Manager.Get_Job();
        }
    }


    public void PerformTask(Task task)
    {
        if (task.Prerequisites != null)
        {
            foreach (Task prerequisite in task.Prerequisites)
            {
                if (prerequisite.TaskStatus != Status.Completed)
                {
                    prerequisite.Execute(this);
                }
            }
        }
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



    #region Basic Task Methods
    public void MoveTo(Vector3 position, Action callback = null)
    {
        // Get the path from the current position to the target position
        List<PathFinding.Node> path = PathFinding.Path.FindPath(current_world.GetMap(), current_world.GetTileAtPosition(transform.position), current_world.GetTileAtPosition(position), true);

        // If the path is null, throw a warning and return
        if (path == null)
        {
            Debug.LogWarning($"Path from {transform.position} to {position} is null");
            return;
        }

        // Start the coroutine to move the pawn
        StartCoroutine(MoveAlongPath(path, callback));
    }

    private IEnumerator MoveAlongPath(List<PathFinding.Node> path, Action callback = null)
    {
        foreach (PathFinding.Node node in path)
        {
            // Move towards each point in the path
            while (Vector3.Distance(transform.position, node.tileObject.worldPos) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, node.tileObject.worldPos, speed * Time.deltaTime);
                yield return null;
            }
        }

        callback?.Invoke();
    }
    #endregion


}


