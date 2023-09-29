using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldGeneration;

/// <summary>
/// A class that manages a queue of jobs and executes them in order of priority.
/// </summary>
public class JobHandler
{
    private Queue<Job> jobQueue = new Queue<Job>();
    private Job currentJob;
    private bool isRunning = false;
    private Pawn pawn;

    public JobHandler(Pawn pawn)
    {
        this.pawn = pawn;
    }

    public void AddJob(Job job)
    {
        jobQueue.Enqueue(job);
    }

    public void UpdateJobs(World world)
    {
        if (jobQueue.Count > 0 && !isRunning)
        {
            currentJob = jobQueue.Dequeue();
            isRunning = true;
            currentJob.isRunning = true;
            currentJob.isAssigned = true;
            pawn.StartCoroutine(currentJob.RunJob(world, pawn, OnJobComplete, OnJobCancelled));
        }
    }

    private void OnJobComplete(Job job)
    {
        Debug.Log($"Job {job.GetType()} is complete.");
        job.isRunning = false;
        job.isComplete = true;
        isRunning = false;
        currentJob = null;
    }
    private void OnJobCancelled(Job job)
    {
        //TODO: Send back to global job handler.
        Debug.Log($"Job {job.GetType()} is cancelled.");
        job.isAssigned = false;
        job.isRunning = false;
        job.isComplete = false;
        isRunning = false;
        currentJob = null;
    }
}

public abstract class Job
{
    public int priority;
    public List<Tile> flags;
    public JobType jobType;
    public bool isRunning;
    public bool isAssigned;
    public bool isComplete;

    public Job(int priority, List<Tile> flags, JobType jobType)
    {
        this.priority = priority;
        this.flags = flags;
        this.jobType = jobType;
    }
    public Job(int priority, Tile flag, JobType jobType)
    {
        this.priority = priority;
        this.flags = new List<Tile>();
        this.flags.Add(flag);
        this.jobType = jobType;
    }
    public abstract IEnumerator RunJob(World world, Pawn pawn, Action<Job> callback, Action<Job> cancelCallback);

    public virtual void OnMovementFinished()
    {

    }
    public virtual void OnMovementCancelled()
    {

    }
}

public enum JobType
{
    Mining,
    Building,
    Hauling,
    Planting,
    Harvesting,
    Cooking,
    Crafting,
    Researching,
    Cleaning,
    Repairing,
    Sleeping,
    Eating,
    Idle
}
