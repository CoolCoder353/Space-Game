using System;
using UnityEngine;
namespace WorldGeneration
{
    [Serializable]
    public class Tile
    {
        public Vector2 position;
        public Vector2 worldPosition;
        public TileType tileType;
        public RockType rockType;
        public GameObject tileObject;

        public Tile objectAbove;
        public Tile objectBelow;
        public bool isRockTile => CheckRockTile();

        public Tile(Vector2 position, Vector2 worldPosition, TileType type, RockType rockType, GameObject tileObject = null)
        {
            this.position = position;
            this.worldPosition = worldPosition;
            this.tileType = type;
            this.rockType = rockType;
            this.tileObject = tileObject;
        }
        public Tile(Vector2 position, Vector2 worldPosition, TileType type, GameObject tileObject = null)
        {
            if (type == TileType.Rock)
            {
                throw new Exception("TileType.Rock needs a rockType");
            }
            this.position = position;
            this.worldPosition = worldPosition;
            this.tileType = type;
            this.tileObject = tileObject;
            this.rockType = RockType.None;
        }

        private bool CheckRockTile()
        {
            if (tileType == TileType.Rock || tileType == TileType.Rock_Smooth)
            {
                return true;
            }
            return false;
        }


    }

    public enum TileType
    {
        None,
        Rock,
        Rock_Smooth,
        Grass,
        Sand,
        Water
    }

    public enum RockType
    {
        None,
        Granite,
        Marble
    }

}