using ElectricityShop.Application.Common.Interfaces;
using Hangfire;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ElectricityShop.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Hangfire implementation of IBackgroundJobService
    /// </summary>
    public class HangfireJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public HangfireJobService(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        /// <summary>
        /// Enqueues a job to be executed in the background
        /// </summary>
        public string EnqueueAsync<T>(Expression<Func<T, Task>> jobExpression)
        {
            return _backgroundJobClient.Enqueue<T>(jobExpression);
        }

        /// <summary>
        /// Schedules a job to be executed at a specific time
        /// </summary>
        public string ScheduleAsync<T>(Expression<Func<T, Task>> jobExpression, TimeSpan delay)
        {
            return _backgroundJobClient.Schedule<T>(jobExpression, delay);
        }

        /// <summary>
        /// Schedules a job to be executed at a specific time
        /// </summary>
        public string ScheduleAsync<T>(Expression<Func<T, Task>> jobExpression, DateTimeOffset enqueueAt)
        {
            return _backgroundJobClient.Schedule<T>(jobExpression, enqueueAt);
        }

        /// <summary>
        /// Enqueues a job to be executed in the background with a specific ID
        /// </summary>
        public bool ContinueJobWith<T>(string parentJobId, Expression<Func<T, Task>> jobExpression)
        {
            return _backgroundJobClient.ContinueJobWith<T>(parentJobId, jobExpression);
        }

        public string AddOrUpdateRecurringJob<T>(string recurringJobId, Expression<Func<T, Task>> jobExpression, string cronExpression)
        {
            _recurringJobManager.AddOrUpdate<T>(recurringJobId, jobExpression, cronExpression);
            return recurringJobId;
        }
    }
}