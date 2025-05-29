using ElectricityShop.Application.Common.Interfaces;
using Hangfire;
using System.Linq.Expressions;

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
        public string Enqueue<T>(Func<T, Task> methodCall)
        {
            // Convert Func to Expression for Hangfire
            Expression<Func<T, Task>> expression = t => methodCall(t);
            return _backgroundJobClient.Enqueue<T>(expression);
        }

        /// <summary>
        /// Schedules a job to be executed at a specific time
        /// </summary>
        public string Schedule<T>(Func<T, Task> methodCall, TimeSpan delay)
        {
            // Convert Func to Expression for Hangfire
            Expression<Func<T, Task>> expression = t => methodCall(t);
            return _backgroundJobClient.Schedule<T>(expression, delay);
        }

        /// <summary>
        /// Enqueues a job to be executed in the background with a specific ID
        /// </summary>
        public bool ContinueJobWith<T>(string parentJobId, Func<T, Task> methodCall)
        {
            // Convert Func to Expression for Hangfire
            Expression<Func<T, Task>> expression = t => methodCall(t);
            return _backgroundJobClient.ContinueJobWith<T>(parentJobId, expression);
        }

        /// <summary>
        /// Schedules a job to be executed at a specific time
        /// </summary>
        public string ScheduleAsync<T>(Expression<Func<T, Task>> jobExpression, DateTimeOffset enqueueAt)
        {
            return _backgroundJobClient.Schedule<T>(jobExpression, enqueueAt);
        }

        public string AddOrUpdateRecurringJob<T>(string recurringJobId, Expression<Func<T, Task>> jobExpression, string cronExpression)
        {
            _recurringJobManager.AddOrUpdate<T>(recurringJobId, jobExpression, cronExpression);
            return recurringJobId;
        }
    }
}