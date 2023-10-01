using UnityEngine;
using System;
using System.Collections;
using WorldGeneration;
using System.Collections.Generic;
using Unity.VisualScripting;
public class MiningJob : Job
{
    bool finishedMoving = true;
    Tile target = null;
    public MiningJob(JobType jobType, Pawn pawn, World world, List<Tile> targets) : base(JobType.Mining, pawn, world, targets)
    {
    }

    public override IEnumerator ExecuteJob()
    {
        while (cancel != true && (targets.Count > 0 || target != null))
        {
            if (targets.Count > 0 && target == null)
            {
                target = targets[0];
                targets.RemoveAt(0);
                finishedMoving = false;
                Debug.Log($"Starting to move to tile {target.position.x}, {target.position.y}.");
                pawn.StartMove(target, world, OnFinishedMoving, OnCancelledMoving);
            }
            else if (target == null)
            {
                Debug.Log("No target to mine. Finished job.");
                //Finish Job
            }
            else if (finishedMoving == true)
            {
                Debug.Log($"Mining tile {target.position.x}, {target.position.y}.");
                world.DamageTile(target, pawn.skillHandler.GetSkill(SkillType.Mining).level + 1);

                if (target.tileObject == null)
                {
                    Debug.Log($"Tile destroyed at {target.position.x}, {target.position.y}.");
                    target = null;

                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        yield return null;
    }


    private void OnCancelledMoving()
    {
        Debug.LogWarning("Movement cancelled. Mining job cancelled.");
        pawn.jobHandler.CancelCurrentJob();
    }

    private void OnFinishedMoving()
    {
        Debug.Log("Finished moving to tile.");
        finishedMoving = true;
    }
}

public class BuildingJob : Job
{
    public BuildingJob(JobType jobType, Pawn pawn, World world, List<Tile> targets) : base(JobType.Building, pawn, world, targets)
    {
    }

    public override IEnumerator ExecuteJob()
    {
        throw new NotImplementedException();
    }
}
