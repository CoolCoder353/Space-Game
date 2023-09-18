using System;

// An enum that represents the name of a skill
/*
public enum NeedName
{
    Food,
    Rest,
    Social,
    Recreation
}
*/
public enum NeedName
{
    Food,
    Rest
}


// An abstract class that represents a need
public abstract class Need
{
    // A property that stores the needs name
    public NeedName Name { get; private set; }

    // A property that stores the needs amount
    public float amount { get; private set; }

    // A property that stores the needs max amount
    public float max { get; private set; }
    // A property that stores the needs remove amount
    public float removeAmount { get; private set; }



    /// <summary>
    /// Initializes a new instance of the <see cref="Need"/> class.
    /// </summary>
    /// <param name="name">The name of the need.</param>
    /// <param name="max">The maximum amount of the need.</param>
    /// <param name="removeAmount">The amount to remove.</param>
    public Need(NeedName name, float max, float removeAmount)
    {
        Name = name;
        amount = max;
        this.max = max;
        this.removeAmount = removeAmount;
    }

    /// <summary>
    /// Sets the amount to remove.
    /// </summary>
    /// <param name="amount">The amount to remove.</param>
    public void SetRemoveAmount(float amount)
    {
        removeAmount = amount;
    }
    /// <summary>
    /// Updates the need by removing the specified amount.
    /// </summary>
    public void Update()
    {
        Remove(removeAmount);
    }
    /// <summary>
    /// Removes the specified amount from the need.
    /// </summary>
    /// <param name="amount">The amount to remove.</param>
    public void Remove(float remove)
    {
        this.amount -= remove;
        ApplyNeedEffect(amount);
    }
    /// <summary>
    /// Adds the specified amount to the need.
    /// </summary>
    /// <param name="amount">The amount to add.</param>
    public void Add(float remove)
    {
        this.amount += remove;
        ApplyNeedEffect(amount);
    }
    // <summary>
    /// Sets the need to the specified amount.
    /// </summary>
    /// <param name="amount">The new value of the need.</param>
    public void Set(float remove)
    {
        this.amount = remove;
        ApplyNeedEffect(amount);
    }


    public abstract void ApplyNeedEffect(float amount);

}

public class NeedHandler
{
    private Need[] needs;
    /// <summary>
    /// Initializes a new instance of the <see cref="NeedHandler"/> class with the specified need names.
    /// </summary>
    /// <param name="needNames">The array of need names.</param>
    public NeedHandler(NeedName[] needNames)
    {
        needs = new Need[needNames.Length];
        foreach (NeedName need in needNames)
        {
            Type type = Type.GetType(need.ToString() + "Need");
            needs[(int)need] = (Need)Activator.CreateInstance(type);
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="NeedHandler"/> class with all available need names.
    /// </summary>
    public NeedHandler()
    {
        needs = new Need[Enum.GetNames(typeof(NeedName)).Length];
        foreach (NeedName need in (NeedName[])Enum.GetValues(typeof(NeedName)))
        {
            Type type = Type.GetType(need.ToString() + "Need");
            needs[(int)need] = (Need)Activator.CreateInstance(type);
        }
    }
    /// <summary>
    /// Returns the need with the specified name.
    /// </summary>
    /// <param name="need">The name of the need.</param>
    public Need GetNeed(NeedName need)
    {
        return needs[(int)need];
    }
    /// <summary>
    /// Removes the need with the specified name.
    /// </summary>
    /// <param name="need">The name of the need to remove.</param>
    public void RemoveNeed(NeedName need)
    {
        needs[(int)need] = null;
    }
    /// <summary>
    /// Adds a new instance of the specified need.
    /// </summary>
    /// <param name="need">The name of the need to add.</param>
    public void AddNeed(NeedName need)
    {
        Type type = Type.GetType(need.ToString() + "Need");
        needs[(int)need] = (Need)Activator.CreateInstance(type);
    }
    /// <summary>
    /// Updates all needs.
    /// </summary>
    public void UpdateNeeds()
    {
        foreach (Need need in needs)
        {
            if (need != null)
            {
                need.Update();
            }
        }
    }
}


