using UnityEngine;
// A class that represents a shooting skill, inherits from Skill class
public class ShootingSkill : Skill
{

    // A constructor that calls the base constructor with the shooting skill name
    public ShootingSkill() : base(SkillType.Shooting)
    {

    }

    // An override method that defines the shooting skill effect
    public override void ApplySkillEffect(int level)
    {
        //TODO: Make this do something.
        Debug.Log($"Shooting skill has increased to level {level}");

    }
}

// A class that represents a melee skill, inherits from Skill class
public class MeleeSkill : Skill
{

    // A constructor that calls the base constructor with the melee skill name
    public MeleeSkill() : base(SkillType.Melee)
    {

    }

    // An override method that defines the melee skill effect
    public override void ApplySkillEffect(int level)
    {
        //TODO: Make this do something.
        Debug.Log($"Melee skill has increased to level {level}");
    }
}

// A class that represents a construction skill, inherits from Skill class
public class ConstructionSkill : Skill
{

    // A constructor that calls the base constructor with the construction skill name
    public ConstructionSkill() : base(SkillType.Construction)
    {

    }

    // An override method that defines the construction skill effect
    public override void ApplySkillEffect(int level)
    {
        //TODO: Make this do something.
        Debug.Log($"Construction skill has increased to level {level}");
    }
}

// A class that represents a construction skill, inherits from Skill class
public class MiningSkill : Skill
{

    // A constructor that calls the base constructor with the construction skill name
    public MiningSkill() : base(SkillType.Mining)
    {

    }

    // An override method that defines the construction skill effect
    public override void ApplySkillEffect(int level)
    {
        //TODO: Make this do something.
        Debug.Log($"Mining skill has increased to level {level}");
    }
}