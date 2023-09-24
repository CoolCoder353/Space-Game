using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NaughtyAttributes;
using UnityEngine;
using WorldGeneration;

public class Pawn : I_Moveable
{
    public SkillHandler skillHandler { get; protected set; }
    public JobHandler globalJobHandler { get; protected set; }
    public JobHandler selfJobHandler { get; protected set; }
    public NeedHandler needHandler { get; protected set; }

    private bool isControllable = true;

    private World world;
    private Tile[,] map;

    private Job currentJob;





    private void Start()
    {
        world = FindObjectOfType<World>();
        map = world.GetMap();
        skillHandler = new();
        globalJobHandler = new();
        selfJobHandler = new();
        needHandler = new();

    }



    public Vector3 miningJobPos = new();
    public Vector3 buildingJobPos = new();


    [Button()]
    public void Debug_Add_Job()
    {
        globalJobHandler.AddJob(new MiningJob(UnityEngine.Random.Range(1, 11), world.GetTileAtPosition(miningJobPos)));
        globalJobHandler.AddJob(new BuildingJob(UnityEngine.Random.Range(1, 11), world.GetTileAtPosition(buildingJobPos)));
    }

    public void AddJob(Job job)
    {
        Debug.Log($"Pawn has a new job: {job.GetType()}");
        globalJobHandler.AddJob(job);
    }

    public bool IsControllable()
    {
        return isControllable;
    }

    void Update()
    {
        needHandler.UpdateNeeds();

        //TODO: Update self jobs based on needs


        if (currentJob == null && (globalJobHandler.HasJob || selfJobHandler.HasJob))
        {
            if (selfJobHandler.HasJob)
            {
                currentJob = selfJobHandler.Get_Job();
            }
            else if (globalJobHandler.HasJob)
            {
                currentJob = globalJobHandler.Get_Job();
            }

            Debug.Log($"Pawn has a new job: {currentJob.GetType()}");
            StartMove(Pathfinder.FindPath(world.GetTileAtPosition(transform.position), currentJob.position, map), 0.1f);

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
