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

                PriorityQueue<TileNode> moveableTiles = new PriorityQueue<TileNode>(8, (x, y) => x.priority.CompareTo(y.priority));
                Tile moveTile = null;
                foreach (Tile neighbours in Pathfinder.GetNeighbors(target, world.GetFloor()))
                {
                    if (neighbours.walkable == true)
                    {
                        moveableTiles.Enqueue(new TileNode(neighbours, -Vector3.Distance(neighbours.worldPosition, pawn.transform.position)));
                    }
                }
                if (moveableTiles.Count > 0)
                {
                    moveTile = moveableTiles.Dequeue().tile;
                }
                else
                {
                    Debug.LogWarning("No moveable tiles found. Cancelling job.");
                    pawn.jobHandler.CancelCurrentJob();
                    yield break;
                }
                targets.RemoveAt(0);
                finishedMoving = false;
                pawn.StartMove(moveTile, world, OnFinishedMoving, OnCancelledMoving);
            }
            else if (target == null)
            {
                Debug.Log("No target to mine. Finished job.");
                //Finish Job
            }
            else if (finishedMoving == true)
            {
                if (target.tileObject != null)
                {
                    bool destroyed = world.DamageTile(target, pawn.skillHandler.GetSkill(SkillType.Mining).level + 1);
                    if (destroyed == true)
                    {
                        target = null;
                    }
                }
                else
                {
                    Debug.LogWarning("Target tile has no object. Cancelling job.");
                    pawn.jobHandler.CancelCurrentJob(false);
                    yield break;
                }



                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForEndOfFrame();
        }
        pawn.jobHandler.FinishCurrentJob();
    }


    public void OnCancelledMoving()
    {

        pawn.jobHandler.CancelCurrentJob();
    }

    public void OnFinishedMoving()
    {
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
