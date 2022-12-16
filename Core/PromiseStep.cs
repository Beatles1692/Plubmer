namespace Plumber.Core
{
    public class PromiseStep<TContext>
    {
        public PromiseStep(Func<TContext, Task> success)
        {
            Success = success;
        }
        public PromiseStep(Func<TContext, Task> success, Func<TContext, Task>? fail)
        {
            Success = success;
            Fail = fail;
        }

        public Func<TContext, Task> Success { get; }
        public Func<TContext, Task>? Fail { get; }

        public async Task<PromiseState> ResolveAsync(TContext context)
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


