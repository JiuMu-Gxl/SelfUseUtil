using Hangfire;
using HangFire.Demo.Job;

namespace HangFire.Demo.Application
{
    public interface IJobService {
        Task AddJob(string cronExpression, string name);
    }

    public class JobService : IJobService
    {
        public async Task AddJob(string cronExpression,string name) {
            // 使用 Hangfire 提供的 RecurringJobExtensions 来添加定时任务
            RecurringJob.AddOrUpdate(name, () => TestJob.ExecuteJob(name), cronExpression);
        }
    }
}
