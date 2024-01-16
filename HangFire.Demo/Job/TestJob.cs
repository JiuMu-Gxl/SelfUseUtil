namespace HangFire.Demo.Job
{
    public class TestJob
    {
        public static void ExecuteJob(string name)
        {
            Console.WriteLine($"[{DateTime.Now}] {name}-定时任务执行中...");
        }
    }
}
