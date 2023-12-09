using UnityEngine;
using System.Collections.Generic;

namespace Tasks
{
    public class global_job_manager : MonoBehaviour
    {
        //A dictionary that contains all the jobs sorted by the type of job they are.
        private Dictionary<string, List<Task>> jobs = new Dictionary<string, List<Task>>();

        public void Add_Job(Task job)
        {
            if (jobs.ContainsKey(job.GetType().ToString()))
            {
                jobs[job.GetType().ToString()].Add(job);
            }
            else
            {
                jobs.Add(job.GetType().ToString(), new List<Task>());
                jobs[job.GetType().ToString()].Add(job);
            }
        }
        public Task Get_Job(string job_type)
        {
            if (jobs.ContainsKey(job_type))
            {
                if (jobs[job_type].Count > 0)
                {
                    Task job = jobs[job_type][0];
                    jobs[job_type].RemoveAt(0);
                    return job;
                }
            }
            return null;
        }

        public Task Get_Job()
        {
            foreach (KeyValuePair<string, List<Task>> job_type in jobs)
            {
                if (job_type.Value.Count > 0)
                {
                    Task job = job_type.Value[0];
                    job_type.Value.RemoveAt(0);
                    return job;
                }
            }
            return null;
        }

    }
}