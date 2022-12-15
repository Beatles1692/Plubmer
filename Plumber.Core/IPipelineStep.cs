namespace Plumber.Core
{
    public interface IPipelineStep<TContext>
    {
        Task ExecuteAsync(TContext context, Func<TContext, Task> next);
    }
}
