namespace Plumber.Core
{
    /// <summary>
    /// A Promise class to chain async tasks
    /// </summary>
    /// <typeparam name="TContext"> type of the context </typeparam>
    public class Promise<TContext>
    {
        private readonly LinkedList<PromiseStep<TContext>> _steps = new LinkedList<PromiseStep<TContext>>();

        private Func<TContext, Task>? _catch;

        /// <summary>
        /// Creates a new Promise
        /// </summary>
        /// <param name="success">the success function</param>
        /// <param name="fail">the fail function</param>
        public Promise(Func<TContext, Task> success, Func<TContext, Task> fail)
        {
            _steps.AddLast(new PromiseStep<TContext>(success, fail));
        }

        /// <summary>
        /// Creates a new Promise
        /// </summary>
        /// <param name="success">the success function</param>
        public Promise(Func<TContext, Task> success)
        {
            _steps.AddLast(new PromiseStep<TContext>(success));
        }

        /// <summary>
        /// Adds the next step to the promise
        /// </summary>
        /// <param name="success">the success function </param>
        /// <param name="fail">the fail function </param>
        /// <returns></returns>
        public Promise<TContext> Then(Func<TContext, Task> success, Func<TContext, Task> fail)
        {
            _steps.AddLast(new PromiseStep<TContext>(success, fail));
            return this;
        }

        /// <summary>
        /// Adds the next step to the promise
        /// </summary>
        /// <param name="success">the success function </param>
        /// <returns></returns>
        public Promise<TContext> Then(Func<TContext, Task> success)
        {
            _steps.AddLast(new PromiseStep<TContext>(success));
            return this;
        }

        /// <summary>
        /// Adds the catch function to the promise
        /// </summary>
        /// <param name="catch">the catch function</param>
        /// <returns></returns>
        public Promise<TContext> Catch(Func<TContext, Task> @catch)
        {
            _catch = @catch;
            return this;
        }


        /// <summary>
        /// Tries to resolve the promise and returns the state
        /// </summary>
        /// <param name="context">the context object</param>
        /// <returns>the last state of the Promise</returns>
        public async Task<PromiseState> ResolveAsync(TContext context)
        {
            PromiseState state = PromiseState.Pending;
            foreach (var step in _steps)
            {
                try
                {
                    state = await step.ResolveAsync(context);
                    if (state == PromiseState.Rejected)
                        break;
                }
                catch (Exception)
                {
                    if (_catch != null)
                    {
                        state = PromiseState.Rejected;
                        await _catch(context);
                    }
                    else
                        throw;
                }
            }
            return state;
        }
    }
}