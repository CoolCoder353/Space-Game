using UnityEngine;
// A class that represents a food need, inherits from Need class
public class FoodNeed : Need
{
    public FoodNeed() : base(NeedName.Food, 100, 2)
    {
    }

    public override void ApplyNeedEffect(float amount)
    {
        //// Debug.Log($"Food need has changed by {amount}");
    }


}

// A class that represents a food need, inherits from Need class
public class RestNeed : Need
{
    public RestNeed() : base(NeedName.Rest, 100, 1)
    {
    }

    public override void ApplyNeedEffect(float amount)
    {
        ////Debug.Log($"Rest need has changed by {amount}");
    }

}