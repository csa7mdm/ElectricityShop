using System;
using System.Threading.Tasks;

namespace ElectricityShop.Application.Common.Interfaces
{
    /// <summary>
    /// Service for managing background jobs
    /// </summary>
    public interface IBackgroundJobService
    {
        /// <summary>
        /// Enqueues a job to be executed in the background
        /// </summary>
        string Enqueue<T>(Func<T, Task> methodCall);
        
        /// <summary>
        /// Schedules a job to be executed at a specific time
        /// </summary>
        string Schedule<T>(Func<T, Task> methodCall, TimeSpan delay);
        
        /// <summary>
        /// Enqueues a job to be executed in the background with a specific ID
        /// </summary>
        bool ContinueJobWith<T>(string parentJobId, Func<T, Task> methodCall);
    }
}