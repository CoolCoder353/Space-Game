using NaughtyAttributes;
using UnityEngine;
using WorldGeneration;

public class Pawn : I_Moveable
{
    public SkillHandler skillHandler { get; protected set; }
    public JobHandler globalJobHandler { get; protected set; }
    public NeedHandler needHandler { get; protected set; }

    private bool isControllable = true;

    private World world;
    private Tile[,] map;

    private Job currentJob;





    private void Start()
    {
        world = FindObjectOfType<World>();
        map = world.GetMap();
        skillHandler = new();
        globalJobHandler = new(this);
        needHandler = new();

    }


    public void AddJob(Job job)
    {
        Debug.Log($"Pawn has a new job: {job.GetType()}");
        globalJobHandler.AddJob(job);
    }

    public bool IsControllable()
    {
        return isControllable;
    }

    void Update()
    {
        needHandler.UpdateNeeds();

        //TODO: Update self jobs based on needs

        globalJobHandler.UpdateJobs(world);
    }


    //    MOVEMENT    \\
    protected override void OnMoveCancelled()
    {
        Debug.Log("Pawn cancelled movement.");
        currentJob?.OnMovementCancelled();
    }

    protected override void OnMoveFinished()
    {
        Debug.Log("Pawn finished moving");
        currentJob?.OnMovementFinished();

    }



}
