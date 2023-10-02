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
    public bool isWorking => currentJob != null;

    public Job currentJob = null;

    public List<Job> jobs = new List<Job>();

    public Global_job_handler global_Job_Handler;

    public JobHandler(Global_job_handler global_Job_Handler)
    {
        this.global_Job_Handler = global_Job_Handler;
    }



    public void AddJob(Job job)
    {
        Debug.Log($"Adding job {job.jobType} to {job.pawn.name}'s job queue.");
        jobs.Add(job);
    }
    public void CancelCurrentJob(bool returnToJobQueue = true)
    {
        currentJob.cancel = true;
        if (returnToJobQueue)
        {
            global_Job_Handler.AddJob(currentJob);
        }
        currentJob = null;
    }
    public void CancelAllJobs()
    {
        jobs.Clear();
        currentJob = null;
    }

    public void GetNextJob()
    {
        if (jobs.Count > 0)
        {
            currentJob = jobs[0];
            currentJob.cancel = false;
            jobs.RemoveAt(0);
            currentJob.pawn.StartCoroutine(currentJob.ExecuteJob());
        }
    }

    public void FinishCurrentJob()
    {
        currentJob = null;
    }
}

public abstract class Job
{
    public JobType jobType;
    public Pawn pawn;
    public World world;

    public List<Tile> targets = new List<Tile>();

    public bool cancel = false;

    public Job(JobType jobType, Pawn pawn, World world, List<Tile> targets)
    {
        this.jobType = jobType;
        this.pawn = pawn;
        this.world = world;
        this.targets = targets;
    }

    public abstract IEnumerator ExecuteJob();

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
