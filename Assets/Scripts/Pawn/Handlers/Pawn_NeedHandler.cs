using System;
using System.Collections.Generic;

// An enum that represents the type of a need

public enum NeedType
{
    Food,
    Rest,
    Social,
    Recreation,
    Chemicals
}


// An abstract class that represents a need
public class Need
{
    public NeedType needType;

    public float maxValue;
    public float currentValue;

    public float decayRate;

    public List<float> thresholds = new List<float>();
    Action<Need> OnNeedAtThreshold;

    public float percentage => currentValue / maxValue;

    public Need(NeedType needType, float maxValue, float currentValue, float decayRate, List<float> thresholds, Action<Need> OnNeedAtThreshold)
    {
        this.needType = needType;
        this.maxValue = maxValue;
        this.currentValue = currentValue;
        this.decayRate = decayRate;
        this.thresholds = thresholds;
        this.OnNeedAtThreshold = OnNeedAtThreshold;
    }


    public void UpdateNeed()
    {
        currentValue -= decayRate;
        //Find the lowest threshold that the percentage is below

        foreach (float threshold in thresholds)
        {
            if (percentage <= threshold)
            {
                OnNeedAtThreshold?.Invoke(this);
                return;
            }
        }
    }

    public void AddNeed(float amount)
    {
        currentValue += amount;
        if (currentValue > maxValue)
        {
            currentValue = maxValue;
        }
    }
}
public class NeedHandler
{
    List<Need> needs = new List<Need>();

    Action<Need> OnNeedEmpty;


    public NeedHandler(List<Need> needs)
    {
        this.needs = needs;
    }

    public bool Contains(NeedType needType)
    {
        foreach (Need need in needs)
        {
            if (need.needType == needType)
            {
                return true;
            }
        }
        return false;
    }

    public void AddAmountToNeed(NeedType need, float amount)
    {
        foreach (Need n in needs)
        {
            if (n.needType == need)
            {
                n.AddNeed(amount);
                return;
            }
        }
    }

    public void UpdateNeeds()
    {
        foreach (Need need in needs)
        {
            // Update the need
            need.UpdateNeed();
        }

    }

    public void RemoveNeed(NeedType needType)
    {
        // Remove the need from the list
        foreach (Need need in needs)
        {
            // Check if the need is the one we are looking for
            if (need.needType == needType)
            {
                // Remove the need
                needs.Remove(need);
                return;
            }
        }
    }
    public void AddNeed(NeedType needType, float maxValue, float currentValue, float decayRate, List<float> thresholds, Action<Need> OnNeedAtThreshold)
    {
        // Add the need to the list
        if (Contains(needType) == false)
        {
            needs.Add(new Need(needType, maxValue, currentValue, decayRate, thresholds, OnNeedAtThreshold));
        }
    }
    public void AddNeed(Need need)
    {
        // Add the need to the list
        if (Contains(need.needType) == false)
        {
            needs.Add(need);
        }
    }
}



