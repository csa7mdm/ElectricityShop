using ElectricityShop.Application.Common.Interfaces;
using Hangfire;
using System;
using System.Threading.Tasks;

namespace ElectricityShop.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Hangfire implementation of IBackgroundJobService
    /// </summary>
    public class HangfireJobService : IBackgroundJobService
    {
        /// <summary>
        /// Enqueues a job to be executed in the background
        /// </summary>
        public string Enqueue<T>(Func<T, Task> methodCall)
        {
            return BackgroundJob.Enqueue<T>(methodCall);
        }

        /// <summary>
        /// Schedules a job to be executed at a specific time
        /// </summary>
        public string Schedule<T>(Func<T, Task> methodCall, TimeSpan delay)
        {
            return BackgroundJob.Schedule<T>(methodCall, delay);
        }

        /// <summary>
        /// Enqueues a job to be executed in the background with a specific ID
        /// </summary>
        public bool ContinueJobWith<T>(string parentJobId, Func<T, Task> methodCall)
        {
            return BackgroundJob.ContinueJobWith<T>(parentJobId, methodCall);
        }
    }
}