namespace ERPCronJob.Interfaces
{
    public interface IScopedService
    {
        Task DoWork(CancellationToken cancellationToken);
    }
}
