using NaughtyAttributes;
using UnityEngine;
using WorldGeneration;

public class Pawn : I_Moveable
{
    NeedHandler needHandler;
    SkillHandler skillHandler;
    JobHandler jobHandler;

    private void Update()
    {
        needHandler.UpdateNeeds();

    }
    protected override void OnMoveCancelled()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnMoveFinished()
    {
        throw new System.NotImplementedException();
    }
}
