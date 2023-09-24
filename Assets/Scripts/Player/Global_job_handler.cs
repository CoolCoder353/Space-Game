//A global handler that looks at the users input and sends jobs to the appropriate pawn.

using System.Collections.Generic;
using System.IO;
using NaughtyAttributes;
using UnityEngine;
using WorldGeneration;

public class Global_job_handler : MonoBehaviour
{
    public int priority = 1;
    private World world;
    private Tile[,] map;

    private List<Job> globalJobs = new List<Job>();

    private Pawn[] pawns;

    private void Start()
    {
        world = FindObjectOfType<World>();
        map = world.GetMap();
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
    [Button()]
    public void Test()
    {
        List<Tile> path = Pathfinder.FindPath(world.GetMap()[0, 0], world.GetMap()[10, 10], world.GetMap());
        Debug.Log(path?.Count);

        world.SetTile(new Vector3(0, 0, 0), TileType.Wall_Blueprint);

        List<Tile> secondPath = Pathfinder.FindPath(world.GetMap()[0, 0], world.GetMap()[10, 10], world.GetMap());
        Debug.Log(secondPath?.Count);
    }

    private void On_Input()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 mousPosInverted = new Vector3(mousePos.y, mousePos.x, mousePos.z);
        Tile mouseTile = world.GetTileAtPosition(mousPosInverted);

        Debug.Log($"Mouse position: ({mousPosInverted.x},{mousPosInverted.y}), Mouse tile position: ({mouseTile.GetPosition().x}, {mouseTile.GetPosition().y}), Mouse world position: ({mouseTile.WorldPosition.x}, {mouseTile.WorldPosition.y})");
        //TODO: Add a check to see if the mouse hit a ui element.
        if (mouseTile != null) //if the mouse hit a tile.
        {

            //TODO: add ghost building block at mouse position.
            world.SetTile(mouseTile, TileType.Wall_Blueprint);

            Tile jobTile = world.GetTileAtPosition(mousePos);
            globalJobs.Add(new BuildingJob(priority, jobTile));

            Send_Job_To_Pawn();
        }
    }

    private int Get_Highest_Construction_Skill()
    {
        int highestSkill = 0;
        foreach (Pawn pawn in pawns)
        {
            if (pawn.skillHandler.GetSkill(SkillName.Construction).Level() > highestSkill)
            {
                highestSkill = pawn.skillHandler.GetSkill(SkillName.Construction).Level();
            }
        }
        return highestSkill;
    }

    private void Send_Job_To_Pawn()
    {
        if (pawns.Length == 0)
        {
            Debug.LogError("No pawns found!");
            return;
        }
        if (globalJobs.Count == 0)
        {
            Debug.LogError("No jobs found!");
            return;
        }

        foreach (Pawn pawn in pawns)
        {
            //if the pawns skill in construction is the highest out of all the pawns, send the job to them.
            if (pawn.skillHandler.GetSkill(SkillName.Construction).Level() == Get_Highest_Construction_Skill())
            {
                pawn.AddJob(globalJobs[0]);
                globalJobs.RemoveAt(0);
            }
        }
    }


}