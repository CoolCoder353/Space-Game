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

    public Job currentJob;

    public List<Job> jobs = new List<Job>();

    public void AddJob(Job job)
    {
        jobs.Add(job);
    }
    public void CancelCurrentJob(bool returnToJobQueue = true)
    {
        if (returnToJobQueue)
        {
            jobs.Insert(0, currentJob);
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
            jobs.RemoveAt(0);
        }
    }


}

public class Job
{
    public JobType jobType;
    public Pawn pawn;
    public World world;

    public Job(JobType jobType, Pawn pawn, World world)
    {
        this.jobType = jobType;
        this.pawn = pawn;
        this.world = world;
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
