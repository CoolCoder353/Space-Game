using System;

// An enum that defines the skill names
public enum SkillName
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

// An abstract class that represents a skill
public abstract class Skill
{
    // A property that stores the skill name
    public SkillName Name { get; private set; }

    // A property that stores the skill xp
    public int Xp { get; private set; }

    // A property that stores the previous level of the skill
    public int PreviousLevel { get; private set; }



    // A constructor that takes a skill name and initializes the xp and previous level to 0
    public Skill(SkillName name)
    {
        Name = name;
        Xp = 0;
        PreviousLevel = 0;
    }

    // A method that returns the level of the skill based on the xp
    public int GetLevel()
    {
        return SkillMath.XpToLevel(Xp); // Use the math function to convert xp to level
    }

    // A method that sets the xp of the skill based on the level
    public void SetLevel(int level)
    {
        Xp = SkillMath.LevelToXp(level); // Use the math function to convert level to xp
        ApplySkillEffect(level); // Apply the skill effect after changing xp
        PreviousLevel = GetLevel(); // Update the previous level
    }

    // A method that increases the xp of the skill by a given amount
    public void IncreaseXp(int amount)
    {
        Xp += amount;
        if (GetLevel() > PreviousLevel) // Check if the current level is greater than the previous level
        {
            ApplySkillEffect(GetLevel()); // Apply the skill effect after increasing xp
            PreviousLevel = GetLevel(); // Update the previous level
        }
    }

    // A method that decreases the xp of the skill by a given amount
    public void DecreaseXp(int amount)
    {
        Xp -= amount;
        if (Xp < 0) Xp = 0; // Ensure xp is not negative
        if (GetLevel() < PreviousLevel) // Check if the current level is lower than the previous level
        {
            ApplySkillEffect(GetLevel()); // Apply the skill effect after decreasing xp
            PreviousLevel = GetLevel(); // Update the previous level
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
    public SkillHandler(SkillName[] skillNames)
    {
        skills = new Skill[Enum.GetNames(typeof(SkillName)).Length];
        foreach (SkillName skill in skillNames)
        {
            Type type = Type.GetType(skill.ToString() + "Skill"); // Get the type of the corresponding skill class 
            skills[(int)skill] = (Skill)Activator.CreateInstance(type); // Create an instance of the skill class and store it in the array 
        }
    }

    public SkillHandler()
    {
        foreach (SkillName skill in (SkillName[])Enum.GetValues(typeof(SkillName)))
        {
            Type type = Type.GetType(skill.ToString() + "Skill"); // Get the type of the corresponding skill class 
            skills[(int)skill] = (Skill)Activator.CreateInstance(type); // Create an instance of the skill class and store it in the array 
        }
    }


    // A method that returns the skill object of a given skill name
    public Skill GetSkill(SkillName skill)
    {
        return skills[(int)skill]; // Return the skill object from the array using the enum value as index 
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
