using System.Collections.Generic;
using UnityEngine;

public struct TileData
{
    string tileType;

    string spritePath;

    bool isWalkable;
    float health;

    float fertilty;

    public TileData(string tileType, string spritePath, bool isWalkable, float health, float fertility)
    {
        this.tileType = tileType;
        this.health = health;
        this.fertilty = fertility;
        this.spritePath = spritePath;
        this.isWalkable = isWalkable;
    }
}