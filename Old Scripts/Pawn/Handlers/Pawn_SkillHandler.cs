using System;
using System.Diagnostics;

// An enum that defines the skill names
/*
public enum SkillType
{
    Shooting,
    Melee,
    Construction,
    Mining,
    Cooking,
    Plants,
    Animals,
    Crafting,
    Artistic,
    Medicine,
    Social,
    Intellectual
}
*/
public enum SkillType
{
    Shooting,
    Melee,
    Construction,
    Mining
}


// An abstract class that represents a skill
//TODO: Implement passions for certain skills / jobs.  This will increase the rate at which the skill increases.
public abstract class Skill
{
    // A property that stores the skill name
    public SkillType Name { get; private set; }

    // A property that stores the skill xp
    public int Xp { get; private set; }

    // A property that stores the previous level of the skill
    public int PreviousLevel { get; private set; }

    public int level => Level();

    public bool canDo = true;




    // A constructor that takes a skill name and initializes the xp and previous level to 0
    public Skill(SkillType name, bool canDo = true)
    {
        Name = name;
        Xp = 0;
        PreviousLevel = 0;
        this.canDo = canDo;
    }

    // A method that returns the level of the skill based on the xp
    public int Level()
    {
        return SkillMath.XpToLevel(Xp); // Use the math function to convert xp to level
    }

    // A method that sets the xp of the skill based on the level
    public void SetLevel(int level)
    {
        Xp = SkillMath.LevelToXp(level); // Use the math function to convert level to xp
        ApplySkillEffect(level); // Apply the skill effect after changing xp
        PreviousLevel = Level(); // Update the previous level
    }

    // A method that increases the xp of the skill by a given amount
    public void IncreaseXp(int amount)
    {
        Xp += amount;
        if (Level() > PreviousLevel) // Check if the current level is greater than the previous level
        {
            ApplySkillEffect(Level()); // Apply the skill effect after increasing xp
            PreviousLevel = Level(); // Update the previous level
        }
    }

    // A method that decreases the xp of the skill by a given amount
    public void DecreaseXp(int amount)
    {
        Xp -= amount;
        if (Xp < 0) Xp = 0; // Ensure xp is not negative
        if (Level() < PreviousLevel) // Check if the current level is lower than the previous level
        {
            ApplySkillEffect(Level()); // Apply the skill effect after decreasing xp
            PreviousLevel = Level(); // Update the previous level
        }
    }

    // An abstract method that defines the skill effect
    public abstract void ApplySkillEffect(int level);

}

// A class that represents a skill handler for characters
public class SkillHandler
{

    // An array that stores the skills and their objects, indexed by the enum values
    private Skill[] skills;

    // A constructor that takes an array of skill names and initializes them as skill objects using reflection
    public SkillHandler(SkillType[] skillNames)
    {
        skills = new Skill[skillNames.Length];
        foreach (SkillType skill in skillNames)
        {
            Type type = Type.GetType(skill.ToString() + "Skill"); // Get the type of the corresponding skill class 
            skills[(int)skill] = (Skill)Activator.CreateInstance(type); // Create an instance of the skill class and store it in the array 
        }
    }

    public SkillHandler()
    {
        skills = new Skill[Enum.GetNames(typeof(SkillType)).Length];
        foreach (SkillType skill in Enum.GetValues(typeof(SkillType)))
        {
            Type type = Type.GetType(skill.ToString() + "Skill"); // Get the type of the corresponding skill class 
            skills[(int)skill] = (Skill)Activator.CreateInstance(type); // Create an instance of the skill class and store it in the array 
        }
    }


    // A method that returns the skill object of a given skill name
    public Skill GetSkill(SkillType skill)
    {
        return skills[(int)skill]; // Return the skill object from the array using the enum value as index 
    }

    public bool CanDoSkill(SkillType skill)
    {
        return skills[(int)skill].canDo;
    }

    public void AddXpToSkill(SkillType skill, int amount)
    {
        skills[(int)skill].IncreaseXp(amount);
    }

}

// A static class that contains the math functions for converting between xp and level
// You can change these functions to suit your needs
public static class SkillMath
{

    // A method that converts between xp and level using a math function
    public static int XpToLevel(int xp)
    {
        return (int)Math.Sqrt(xp); // Example: square root function
    }

    // A method that converts between level and xp using a math function
    public static int LevelToXp(int level)
    {
        return level * level; // Example: square function
    }
}
