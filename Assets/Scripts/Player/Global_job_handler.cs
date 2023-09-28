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


    private void On_Input()
    {
        // Check if the mouse is over a UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 mousPosInverted = new Vector3(mousePos.y, mousePos.x, mousePos.z);
        Tile mouseTile = world.GetTileAtPosition(mousePos);
        Tile mouseTileInverted = world.GetTileAtPosition(mousPosInverted);

        ////Debug.Log($"Mouse position: ({mousePos.x},{mousePos.y}), Mouse tile position: ({mouseTile.GetPosition().x}, {mouseTile.GetPosition().y}), Mouse world position: ({mouseTile.WorldPosition.x}, {mouseTile.WorldPosition.y})");
        if (mouseTile != null && mouseTileInverted != null) //if the mouse hit a tile.
        {
            Tile jobTile = world.GetTileAtPosition(mousPosInverted);
            world.SetTile(mouseTileInverted, TileType.Wall_Blueprint, TileLayer.Blueprint);

            //TODO: Send to pawns that are controllable.
            foreach (Pawn pawn in pawns)
            {
                pawn.AddJob(new BuildingJob(priority, jobTile));
            }
        }
    }



}