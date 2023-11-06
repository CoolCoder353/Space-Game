using System;
using Unity.VisualScripting;
using UnityEngine;
namespace WorldGeneration
{
    [Serializable]
    public class Floor : Tile
    {
        public FloorType floorType;
        public RockType rockType;

        public bool isRockTile => CheckRockTile();



        public Item itemOnDeath;
        public int itemAmountOnDeath;

        public float fertility;



        public Floor(Vector2Int position, Vector2 worldPosition, FloorType type, RockType rockType, float maxHealth, int walkSpeed, Item itemOnDeath, int itemAmountOnDeath, float fertility, GameObject tileObject = null) : base(position, worldPosition, maxHealth, walkSpeed, tileObject)
        {
            this.floorType = type;
            this.rockType = rockType;

            this.itemOnDeath = itemOnDeath;
            this.itemAmountOnDeath = itemAmountOnDeath;
            this.fertility = fertility;
        }
        public Floor(Vector2Int position, Vector2 worldPosition, FloorType type, float maxHealth, int walkSpeed, Item itemOnDeath, int itemAmountOnDeath, float fertility, GameObject tileObject = null) : base(position, worldPosition, maxHealth, walkSpeed, tileObject)
        {
            if (type == FloorType.Rock)
            {
                throw new Exception("FloorType.Rock needs a rockType");
            }

            this.floorType = type;
            this.tileObject = tileObject;
            this.rockType = RockType.None;

            this.itemOnDeath = itemOnDeath;
            this.itemAmountOnDeath = itemAmountOnDeath;
            this.fertility = fertility;
        }


        private bool CheckRockTile()
        {
            if (floorType == FloorType.Rock || floorType == FloorType.Rock_Smooth)
            {
                return true;
            }
            return false;
        }
    }

    public enum FloorType
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