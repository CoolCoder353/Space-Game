using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WorldGeneration;

public abstract class I_Movealble : MonoBehaviour
{


    private bool cancelMove = false;

    private int currentPointIndex = 0;

    private float moveSpeed;
    private int maxReccuritionDepth = 1000000;

    private void Start()
    {
        cancelMove = false;
    }

    public void StartMove(List<Tile> tiles, float moveSpeed)
    {
        if (tiles == null)
        {
            Debug.LogWarning("Can't move to null tiles. Exiting");
            return;
        }
        if (tiles.Count <= 0)
        {
            Debug.LogWarning("Can't move to 0 tiles. Exiting");
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
        currentPointIndex = 0;
        cancelMove = false;
        for (int i = 0; i < maxReccuritionDepth; i++)
        {
            Vector2 ourPos = new(transform.position.x, transform.position.y);
            Vector2 targetPos = new(tiles[currentPointIndex].WorldPosition.x, tiles[currentPointIndex].WorldPosition.y);
            if (ourPos != targetPos)
            {
                Vector3 pos = new(targetPos.x, targetPos.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, pos, moveSpeed * Time.deltaTime);
            }
            else
            {
                currentPointIndex++;
                if (currentPointIndex >= tiles.Count)
                {
                    OnMoveFinished();
                    currentPointIndex = 0;
                    yield break;
                }
            }

            // Check if the operation has been cancelled
            if (cancelMove)
            {
                OnMoveCancelled();
                yield break;
            }

            // Wait for some time before moving to the next tile
            yield return new WaitForEndOfFrame();
        }
        Debug.LogError("Max reccurition depth reached. Cancelling move");
        OnMoveCancelled();
    }

    protected abstract void OnMoveFinished();
    protected abstract void OnMoveCancelled();
}
