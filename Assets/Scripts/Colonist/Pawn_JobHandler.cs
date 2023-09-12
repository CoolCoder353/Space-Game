public class JobHandler
{
    protected PriorityQueue<Job> jobs;

    public int maxJobs { get; protected set; }
    public float jobCheckTime { get; protected set; }

    public JobHandler(int maxJobs = 50)
    {
        this.maxJobs = maxJobs;
        ClearJobs();
    }

    public void AddJob(Job job)
    {
        jobs.Enqueue(job);
    }

    public void Run_Job()
    {
        Job job = jobs.Dequeue();
        StartCoroutine(Run(job));
    }

    public IEnumerator Run(Job job)
    {
        job.Job_Start();
        while !job.IsJobComplete()
        {
            job.Job_Update();
        }
        job.Job_Finish();
    }


    public bool HasJob()
    {
        return jobs.Count > 0;
    }

    public void ClearJobs()
    {
        jobs = new PriorityQueue<Job>(maxJobs, (Job a, Job b) => a.priority.CompareTo(b.priority));
    }
}

public abstract class Job
{
    public int priority;
    public WorldGeneration.Tile position;
    public JobType type;

    public Job(int priority, WorldGeneration.Tile position, JobType type)
    {
        this.priority = priority;
        this.position = position;
        this.type = type;
    }

    public void Job_Start()
    {

    }
    public void Job_Update()
    {

    }
    public void Job_Finish()
    {

    }
    public abstract bool IsJobComplete();
}
public enum JobType
{
    Mining,
    Building,
    Hauling,
    Planting,
    Harvesting,
    Cooking,
    Crafting,
    Researching,
    Cleaning,
    Repairing,
    Sleeping,
    Eating,
    Idle
}