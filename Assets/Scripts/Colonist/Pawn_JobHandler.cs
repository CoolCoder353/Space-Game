/// <summary>
/// A class that manages a queue of jobs and executes them in order of priority.
/// </summary>
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

    /// <summary>
    /// Adds a job to the queue.
    /// </summary>
    /// <param name="job">The job to add.</param>
    public void AddJob(Job job)
    {
        jobs.Enqueue(job);
    }

    /// <summary>
    /// Dequeues and runs the next job in the queue.
    /// </summary>
    public void Run_Job()
    {
        var job = jobs.Dequeue();
        // TODO: Implement job execution logic here.
    }

    /// <summary>
    /// Returns true if there are any jobs in the queue, false otherwise.
    /// </summary>
    public bool HasJob => jobs.Count > 0;

    /// <summary>
    /// Clears all jobs from the queue.
    /// </summary>
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

    /// <summary>
    /// Returns true if the job is complete, false otherwise.
    /// </summary>
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
