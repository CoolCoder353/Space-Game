using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using WorldGeneration;

public class Pawn : I_Moveable
{
    public Pathfinder pathfinder;
    private SkillHandler skillHandler;
    private JobHandler jobHandler;
    private NeedHandler needHandler;

    private World world;
    private Tile[,] map;

    private Job currentJob;


    private void Start()
    {
        world = FindObjectOfType<World>();
        map = world.GetMap();
        skillHandler = new();
        pathfinder = new(map);
        jobHandler = new();
        needHandler = new();

    }

    public Vector3 miningJobPos = new();
    public Vector3 buildingJobPos = new();


    [Button()]
    public void Debug_Add_Job()
    {
        jobHandler.AddJob(new MiningJob(UnityEngine.Random.Range(1, 11), world.GetTileAtPosition(miningJobPos)));
        jobHandler.AddJob(new BuildingJob(UnityEngine.Random.Range(1, 11), world.GetTileAtPosition(buildingJobPos)));
    }

    void Update()
    {
        needHandler.UpdateNeeds();
        if (currentJob == null && jobHandler.HasJob)
        {

            currentJob = jobHandler.Get_Job();
            Debug.Log($"Pawn has a new job: {currentJob.GetType()}");
            StartMove(pathfinder.FindPath(world.GetTileAtPosition(transform.position), currentJob.position), 0.1f);
        }
    }

    public void JobFinished(Job job)
    {
        Debug.Log($"Job {job.GetType()} finished");
        currentJob = null;
    }

    //    MOVEMENT    \\
    protected override void OnMoveCancelled()
    {
        Debug.Log("Pawn cancelled movement. Skipping to next job");
        currentJob = null;
    }

    protected override void OnMoveFinished()
    {
        Debug.Log("Pawn finished moving");
        StartCoroutine(currentJob.RunJob(JobFinished));

    }



}
