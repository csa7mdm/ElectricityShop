using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using System;

namespace ElectricityShop.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Global Hangfire error handler for better error handling and reporting
    /// </summary>
    public class HangfireErrorHandler : JobFilterAttribute, IServerFilter, IApplyStateFilter
    {
        private readonly ILogger<HangfireErrorHandler> _logger;
        
        /// <summary>
        /// Initializes a new instance of the HangfireErrorHandler
        /// </summary>
        public HangfireErrorHandler(ILogger<HangfireErrorHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        /// <summary>
        /// Called before a job is executed
        /// </summary>
        public void OnPerforming(PerformingContext filterContext)
        {
            // Do nothing on job start
        }
        
        /// <summary>
        /// Called after a job is executed
        /// </summary>
        public void OnPerformed(PerformedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                // Log detailed job failure information
                _logger.LogError(filterContext.Exception, 
                    "Background job failed. Job ID: {JobId}, Job Type: {JobType}, Method: {JobMethod}",
                    filterContext.BackgroundJob.Id,
                    filterContext.BackgroundJob.Job.Type.Name,
                    filterContext.BackgroundJob.Job.Method.Name);
                
                // You can implement custom error handling here, such as:
                // - Sending alerts to operations team
                // - Triggering compensation actions for failed jobs
                // - Recording failure metrics
            }
        }
        
        /// <summary>
        /// Called when a state is applied to a job
        /// </summary>
        public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            if (context.NewState is FailedState failedState)
            {
                var job = context.BackgroundJob.Job;
                var jobId = context.BackgroundJob.Id;
                
                // Create more structured job failure data
                var jobFailureInfo = new
                {
                    JobId = jobId,
                    JobType = job.Type.Name,
                    JobMethod = job.Method.Name,
                    Arguments = job.Args,
                    ExceptionMessage = failedState.Exception.Message,
                    ExceptionType = failedState.Exception.GetType().Name,
                    StackTrace = failedState.Exception.StackTrace,
                    FailedAt = DateTime.UtcNow
                };
                
                // Log structured failure information
                _logger.LogError("Background job failed: {@JobFailureInfo}", jobFailureInfo);
                
                // You could store this information in a dedicated table for analysis
            }
        }
        
        /// <summary>
        /// Called when a state is unapplied from a job
        /// </summary>
        public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            // Do nothing when a state is unapplied
        }
    }
}