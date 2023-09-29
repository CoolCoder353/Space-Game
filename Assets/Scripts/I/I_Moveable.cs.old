using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WorldGeneration;
using System.Net;
using System;

public abstract class I_Moveable : MonoBehaviour
{
    public Sprite front;
    public Sprite back;
    public Sprite left;
    public Sprite right;

    private bool cancelMove = false;

    private bool isMoving = false;

    private int currentPointIndex = 0;

    private float moveSpeed;
    private int maxReccuritionDepth = 1000000;

    public void StartMove(Tile tile, World world, float moveSpeed)
    {
        if (tile == null)
        {
            Debug.LogWarning("Can't move to null tile. Exiting");
            OnMoveCancelled();
            return;
        }
        List<Tile> tiles = Pathfinder.FindPath(world.GetTileAtPosition(transform.position), tile, world.GetMap());
        StartMove(tiles, moveSpeed);
    }
    public void StartMove(Tile tile, World world)
    {
        if (tile == null)
        {
            Debug.LogWarning("Can't move to null tile. Exiting");
            OnMoveCancelled();
            return;
        }
        List<Tile> tiles = Pathfinder.FindPath(world.GetTileAtPosition(transform.position), tile, world.GetMap());
        StartMove(tiles, this.moveSpeed);
    }
    public void StartMove(List<Tile> tiles, float moveSpeed)
    {
        if (tiles == null)
        {
            Debug.LogWarning("Can't move to null tiles. Exiting");
            OnMoveCancelled();

            return;
        }
        if (tiles.Count <= 0)
        {
            Debug.LogWarning("Can't move to 0 tiles. Exiting");
            OnMoveCancelled();
            return;
        }
        cancelMove = false;
        currentPointIndex = 0;

        SetMoveSpeed(moveSpeed);
        StartCoroutine(MoveToTiles(tiles));
    }

    protected void SetMoveSpeed(float moveSpeed)
    {
        this.moveSpeed = moveSpeed;
    }

    protected void SetMaxReccuritionDepth(int maxReccuritionDepth)
    {
        this.maxReccuritionDepth = maxReccuritionDepth;
    }

    public void CancelMove()
    {
        cancelMove = true;
    }

    private IEnumerator MoveToTiles(List<Tile> tiles)
    {
        isMoving = true;
        currentPointIndex = 0;
        cancelMove = false;
        for (int i = 0; i < maxReccuritionDepth; i++)
        {
            Vector2 ourPos = new(transform.position.x, transform.position.y);
            Vector2 targetPos = new(tiles[currentPointIndex].WorldPosition.x, tiles[currentPointIndex].WorldPosition.y);
            if (ourPos != targetPos)
            {
                //Get direction of travel 
                Vector2 direction = targetPos - ourPos;

                //Set sprite based on direction
                if (direction.x > 0)
                {
                    GetComponent<SpriteRenderer>().sprite = left;
                }
                else if (direction.x < 0)
                {
                    GetComponent<SpriteRenderer>().sprite = right;
                }
                else if (direction.y > 0)
                {
                    GetComponent<SpriteRenderer>().sprite = back;
                }
                else if (direction.y < 0)
                {
                    GetComponent<SpriteRenderer>().sprite = front;
                }

                Vector3 pos = new(targetPos.x, targetPos.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, pos, moveSpeed * Time.deltaTime);
            }
            else
            {
                currentPointIndex++;
                if (currentPointIndex >= tiles.Count)
                {
                    isMoving = false;
                    OnMoveFinished();
                    currentPointIndex = 0;
                    yield break;
                }
            }

            // Check if the operation has been cancelled
            if (cancelMove)
            {
                isMoving = false;
                OnMoveCancelled();
                yield break;
            }

            // Wait for some time before moving to the next tile
            yield return new WaitForEndOfFrame();
        }
        Debug.LogError("Max reccurition depth reached. Cancelling move");
        OnMoveCancelled();
        isMoving = false;
    }

    protected abstract void OnMoveFinished();
    protected abstract void OnMoveCancelled();
}
