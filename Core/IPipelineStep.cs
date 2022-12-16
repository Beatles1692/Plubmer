namespace Plumber.Core
{
    /// <summary>
    /// An interface to create a pipeline step
    /// </summary>
    /// <typeparam name="TContext">type of Pipeline context</typeparam>
    public interface IPipelineStep<TContext>
    {
        /// <summary>
        /// Executes the step
        /// </summary>
        /// <param name="context">the pipeline context</param>
        /// <param name="next">a reference to next step</param>
        /// <returns></returns>
        Task ExecuteAsync(TContext context, Func<TContext, Task> next);
    }
}
