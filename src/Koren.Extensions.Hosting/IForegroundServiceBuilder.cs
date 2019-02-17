namespace Koren.Extensions.Hosting
{
    public interface IForegroundServiceBuilder
    {
        IForegroundServiceBuilder AddTask<TTask>() where TTask : class,IForegroundTask;
    }
}
