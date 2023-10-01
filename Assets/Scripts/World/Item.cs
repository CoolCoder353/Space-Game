using UnityEngine;

public class Item
{
    public string name;
    public int stackSize;
    public Sprite sprite;

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