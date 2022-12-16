namespace Plumber.Core
{
    public class Promise<TContext>
    {
        private readonly LinkedList<PromiseStep<TContext>> _steps = new LinkedList<PromiseStep<TContext>>();

        private Func<TContext, Task>? _catch;

        public Promise(Func<TContext, Task> success, Func<TContext, Task> fail)
        {
            _steps.AddLast(new PromiseStep<TContext>(success, fail));
        }

        public Promise(Func<TContext, Task> success)
        {
            _steps.AddLast(new PromiseStep<TContext>(success));
        }

        public Promise<TContext> Then(Func<TContext, Task> success, Func<TContext, Task> fail)
        {
            _steps.AddLast(new PromiseStep<TContext>(success, fail));
            return this;
        }

        public Promise<TContext> Then(Func<TContext, Task> success)
        {
            _steps.AddLast(new PromiseStep<TContext>(success));
            return this;
        }

        public Promise<TContext> Catch(Func<TContext, Task> fail)
        {
            _catch = fail;
            return this;
        }


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