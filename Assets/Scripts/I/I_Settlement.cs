using UnityEngine;
using System.Collections.Generic;

public class Settlement
{
    public string Name { get; set; }
    public Pawn leader { get; set; }
    public bool isPlayerSettlement { get; set; }

    public Settlement(string name, bool isPlayerSettlement)
    {
        Name = name;
        this.isPlayerSettlement = isPlayerSettlement;
    }
    public Settlement(string name, bool isPlayerSettlement, Pawn leader)
    {
        Name = name;
        this.isPlayerSettlement = isPlayerSettlement;
        this.leader = leader;
    }
}