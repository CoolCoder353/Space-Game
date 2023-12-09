using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory
{
    Weapon,
    Food,
    Material,
    // Add more categories as needed...
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    // Add more rarities as needed...
}

[Serializable]
public abstract class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Sprite Sprite { get; set; }
    public int StackSize { get; set; }
    public float Weight { get; set; }
    public ItemCategory Category { get; set; }
    public ItemRarity Rarity { get; set; }
    public DateTime? ExpiryDate { get; set; } // Nullable expiry date

    public abstract void Use(Pawn user);
}

public class Food : Item
{
    public int Nutrition { get; set; }

    public override void Use(Pawn user)
    {
        if (ExpiryDate.HasValue && DateTime.Now > ExpiryDate.Value)
        {
            // Implement logic for using expired food
        }
        else
        {
            // Implement logic for using non-expired food
        }
    }
}

public class Weapon : Item
{
    public int Damage { get; set; }

    public override void Use(Pawn user)
    {
        user.Equip(this);
    }
}
public class Inventory
{
    private List<(Item item, int quantity)> items = new List<(Item, int)>();

    public float TotalWeight
    {
        get { return items.Sum(i => i.item.Weight * i.quantity); }
    }

    public void AddItem(Item item, int quantity = 1)
    {
        var existingItem = items.FirstOrDefault(i => i.item.Name == item.Name);
        if (existingItem.item != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            items.Add((item, quantity));
        }
    }

    public bool RemoveItem(Item item, int quantity = 1)
    {
        var existingItem = items.FirstOrDefault(i => i.item.Name == item.Name);
        if (existingItem.item != null && existingItem.quantity >= quantity)
        {
            existingItem.quantity -= quantity;
            if (existingItem.quantity <= 0)
            {
                items.Remove(existingItem);
            }
            return true;
        }
        return false;
    }

    public bool Contains(Item item)
    {
        return items.Any(i => i.item.Name == item.Name);
    }

    public int GetQuantity(Item item)
    {
        var existingItem = items.FirstOrDefault(i => i.item.Name == item.Name);
        return existingItem.item != null ? existingItem.quantity : 0;
    }

    public List<Item> GetItems()
    {
        return items.Select(i => i.item).ToList();
    }
}