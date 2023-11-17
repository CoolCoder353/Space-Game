using System.Collections.Generic;
using UnityEngine;

public struct TileData
{
    public string tileType;

    public string spritePath;


    public bool isWalkable;
    public float health;

    public float fertilty;

    public TileData(string tileType, string spritePath, bool isWalkable, float health, float fertility)
    {
        this.tileType = tileType;
        this.health = health;
        this.fertilty = fertility;
        this.spritePath = spritePath;
        this.isWalkable = isWalkable;
    }
}