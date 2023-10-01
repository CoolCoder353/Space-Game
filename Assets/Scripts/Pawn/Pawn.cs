using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using WorldGeneration;

public class Pawn : I_Moveable
{
    public NeedHandler needHandler;
    public SkillHandler skillHandler;
    public JobHandler jobHandler;

    private void Start()
    {
        List<Need> needs = new List<Need>();
        needs.Add(new Need(NeedType.Food, 100, 50, 1, null, OnFoodAtThreshold));
        needs.Add(new Need(NeedType.Rest, 100, 50, 1, null, OnRestAtThreshold));

        needHandler = new NeedHandler(needs);
        skillHandler = new SkillHandler();
        jobHandler = new JobHandler(FindObjectOfType<Global_job_handler>());
    }

    private void OnFoodAtThreshold(Need need)
    {
        throw new NotImplementedException();
    }

    private void OnRestAtThreshold(Need need)
    {
        throw new NotImplementedException();
    }

    private void Update()
    {
        needHandler.UpdateNeeds();

        if (jobHandler.isWorking == false)
        {
            jobHandler.GetNextJob();
            jobHandler.currentJob.ExecuteJob();
        }

    }
    protected override void OnMoveCancelled()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMoveFinished()
    {
        throw new System.NotImplementedException();
    }
}
