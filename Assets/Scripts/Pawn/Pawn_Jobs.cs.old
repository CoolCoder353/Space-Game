using UnityEngine;
using System;
using System.Collections;
using WorldGeneration;
using System.Collections.Generic;
public class MiningJob : Job
{
    public MiningJob(int priority, List<Tile> flags) : base(priority, flags, JobType.Mining)
    {
    }
    public MiningJob(int priority, Tile flags) : base(priority, flags, JobType.Mining)
    {
    }

    public override IEnumerator RunJob(World world, Pawn pawn, Action<Job> callback, Action<Job> cancelCallback)
    {
        throw new NotImplementedException();
    }
}

public class BuildingJob : Job
{
    bool finishedMoving = true;
    bool cancelled = false;

    public BuildingJob(int priority, List<Tile> flags) : base(priority, flags, JobType.Building)
    {
    }
    public BuildingJob(int priority, Tile flags) : base(priority, flags, JobType.Building)
    {
    }


    public override IEnumerator RunJob(World world, Pawn pawn, Action<Job> callback, Action<Job> cancelCallback)
    {
        pawn.StartMove(flags[0], world);
        finishedMoving = false;
        while (!finishedMoving)
        {
            yield return null;
        }
        if (cancelled)
        {
            cancelCallback(this);
            yield break;
        }
        world.SetTile(flags[0], TileType.Wall, TileLayer.Tile);
        world.RemoveBlueprint(flags[0].GetPosition());
        callback(this);

    }
    public override void OnMovementFinished()
    {
        finishedMoving = true;
    }
    public override void OnMovementCancelled()
    {
        finishedMoving = true;
        cancelled = true;
    }


}
