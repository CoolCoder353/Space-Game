using UnityEngine;

public class Item
{
    public string name;
    public int stackSize;
    public Sprite sprite;

    public bool hasColour = false;
    public Color32 itemColour;
    public int currnetAmount;

    public bool allowed = false;
    public Item(string name, int stackSize, Sprite sprite, bool allowed = false)
    {
        this.name = name;
        this.stackSize = stackSize;
        this.sprite = sprite;
        this.allowed = allowed;
    }
    public Item(Item item)
    {
        this.name = item.name;
        this.stackSize = item.stackSize;
        this.sprite = item.sprite;
        this.allowed = item.allowed;
        this.itemColour = item.itemColour;
        this.hasColour = item.hasColour;
    }

    public void SetColour(Color32 color)
    {
        hasColour = true;
        itemColour = color;
    }
    public void AddAmount(int amount)
    {
        currnetAmount += amount;
    }

    public void RemoveAmount(int amount)
    {
        currnetAmount -= amount;
    }

}