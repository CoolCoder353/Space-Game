using System;
using Unity.VisualScripting;
using UnityEngine;
namespace WorldGeneration
{
    [Serializable]
    public class Tile
    {
        public Vector2Int position;
        public Vector2 worldPosition;
        public TileType tileType;
        public RockType rockType;
        public GameObject tileObject;

        public Tile objectAbove;
        public Tile objectBelow;
        public bool isRockTile => CheckRockTile();

        public bool walkable => isWalkable();

        public int walkSpeed;

        public float maxHealth;
        public float currentHealth;

        public Item itemOnDeath;
        public int itemAmountOnDeath;

        public float fertility;



        public Tile(Vector2Int position, Vector2 worldPosition, TileType type, RockType rockType, float maxHealth, int walkSpeed, Item itemOnDeath, int itemAmountOnDeath, float fertility, GameObject tileObject = null)
        {
            this.position = position;
            this.worldPosition = worldPosition;
            this.tileType = type;
            this.rockType = rockType;
            this.tileObject = tileObject;
            this.maxHealth = maxHealth;
            currentHealth = maxHealth;
            this.walkSpeed = walkSpeed;
            this.itemOnDeath = itemOnDeath;
            this.itemAmountOnDeath = itemAmountOnDeath;
            this.fertility = fertility;
        }
        public Tile(Vector2Int position, Vector2 worldPosition, TileType type, float maxHealth, int walkSpeed, Item itemOnDeath, int itemAmountOnDeath, float fertility, GameObject tileObject = null)
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
            this.maxHealth = maxHealth;
            currentHealth = maxHealth;
            this.walkSpeed = walkSpeed;
            this.itemOnDeath = itemOnDeath;
            this.itemAmountOnDeath = itemAmountOnDeath;
            this.fertility = fertility;
        }


        private bool CheckRockTile()
        {
            if (tileType == TileType.Rock || tileType == TileType.Rock_Smooth)
            {
                return true;
            }
            return false;
        }

        public bool isWalkable()
        {
            if (objectAbove != null)
            {
                return false;
            }
            if (walkSpeed == 0)
            {
                return false;
            }
            return true;

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