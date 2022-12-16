namespace Plumber.Core
{
    /// <summary>
    /// Defines a Promise Step
    /// </summary>
    /// <typeparam name="TContext">type of Promise context</typeparam>
    public class PromiseStep<TContext>
    {
        /// <summary>
        /// Creates a new Promise Step
        /// </summary>
        /// <param name="success">the success function</param>
        public PromiseStep(Func<TContext, Task> success)
        {
            Success = success;
        }
        /// <summary>
        /// Creates a new Promise Step
        /// </summary>
        /// <param name="success">the success function</param>
        /// <param name="fail">the fail function</param>
        public PromiseStep(Func<TContext, Task> success, Func<TContext, Task>? fail)
        {
            Success = success;
            Fail = fail;
        }

        internal Func<TContext, Task> Success { get; }
        internal Func<TContext, Task>? Fail { get; }

        internal async Task<PromiseState> ResolveAsync(TContext context)
        {
            var state = PromiseState.Pending;
            try
            {
                await Success(context);
                state = PromiseState.Resolved;
            }
            catch (Exception)
            {
                if (Fail != null)
                {
                    await Fail(context);
                    state = PromiseState.Rejected;
                }
                else
                    throw;
            }
            return state;
        }
    }
}


