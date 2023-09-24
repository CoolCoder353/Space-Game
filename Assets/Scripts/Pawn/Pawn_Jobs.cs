using UnityEngine;
using System;
using System.Collections;
public class MiningJob : Job
{
    public MiningJob(int priority, WorldGeneration.Tile position) : base(priority, position, JobType.Mining)
    {

    }

    public override IEnumerator RunJob(Action<Job> callback)
    {
        Debug.Log("Mining job is running...");
        yield return new WaitForSeconds(5);
        callback(this);
    }


}

public class BuildingJob : Job
{

    public BuildingJob(int priority, WorldGeneration.Tile position) : base(priority, position, JobType.Building)
    {
    }


    public override IEnumerator RunJob(Action<Job> callback)
    {
        Debug.Log("Building job is running...");
        yield return new WaitForSeconds(10);
        callback(this);
    }

}
