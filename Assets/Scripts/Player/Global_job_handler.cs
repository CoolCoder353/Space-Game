//A global handler that looks at the users input and sends jobs to the appropriate pawn.

using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;
using WorldGeneration;

public class Global_job_handler : MonoBehaviour
{
    public int priority = 1;
    private World world;
    private Tile[,] map;
    private Pawn[] pawns;

    private void Start()
    {
        world = FindObjectOfType<World>();
        map = world.GetFloor();
        //TODO: Get pawns from world instead of finding them.
        //TODO: Get controllable pawns instead of all.

        pawns = FindObjectsOfType<Pawn>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            On_Input();
        }
    }


    private void On_Input()
    {
        // Check if the mouse is over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //When the player clicks on a tile, check if that tile is in the hills layer.
        //If it is, then send a job to the pawn to mine that tile.

        Tile tile = world.GetHillTileAtPosition(mousePos);
        if (tile != null)
        {

            foreach (Pawn pawn in pawns)
            {
                if (pawn.skillHandler.CanDoSkill(SkillType.Mining))
                {
                    pawn.jobHandler.AddJob(new MiningJob(JobType.Mining, pawn, world, new List<Tile>() { tile }));
                    return;
                }
            }
        }



    }

    public void AddJob(Job job)
    {
        Debug.LogError($"Need to add job {job.jobType} but this hasn't been implemented yet.");
    }



}