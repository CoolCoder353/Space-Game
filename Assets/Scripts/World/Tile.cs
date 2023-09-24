using System;
using UnityEngine;
namespace WorldGeneration
{

    [Serializable]
    public class Tile
    {
        private TileType type;
        private Color debugColour;
        private Sprite texture;
        private int speed;
        public (int x, int y) Position { get; set; }
        public (float x, float y) WorldPosition { get; set; }

        public Vector2Int GetPosition()
        {
            return new Vector2Int(Position.x, Position.y);
        }
        public TileType GetTileType()
        {
            return type;
        }

        public Color GetColor()
        {
            return debugColour;
        }

        public Sprite GetTexture()
        {
            return texture;
        }

        public Tile(TileType type, int speed, Color debugColor, Sprite texture, int x, int y, float worldx, float worldy)
        {
            this.type = type;
            this.debugColour = debugColor;
            this.texture = texture;
            this.Position = (x, y);
            this.speed = speed;
            this.WorldPosition = (worldx, worldy);
        }

        public int WalkSpeed()
        {
            return speed;
        }

        public bool Walkable()
        {
            return WalkSpeed() == 0 ? false : true;
        }
    }

    public enum TileType
    {
        Grass,
        Rock,
        Space,
        Wall_Blueprint
    }
}