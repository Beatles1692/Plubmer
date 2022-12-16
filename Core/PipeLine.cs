namespace Plumber.Core;
/// <summary>
/// A Pipeline that accepts a context and executes a series of steps in order on that context
/// </summary>
/// <typeparam name="TContext"></typeparam>
public class PipeLine<TContext>
{
    private readonly LinkedList<Func<TContext, Func<TContext, Task>, Task>> _steps = new LinkedList<Func<TContext, Func<TContext, Task>, Task>>();

    /// <summary>
    /// Adds a step at the end of the pipeline
    /// </summary>
    /// <param name="step"> the step to be added</param>
    /// <returns></returns>
    public PipeLine<TContext> AddNextStep(IPipelineStep<TContext> step) => AddNextStep((context, next) => step.ExecuteAsync(context, next));

    /// <summary>
    /// Adds a step at the end of the pipeline
    /// </summary>
    /// <param name="step"> the step function to be added</param>
    /// <returns></returns>
    public PipeLine<TContext> AddNextStep(Func<TContext, Func<TContext, Task>, Task> step)
    {
        _steps.AddLast(step);
        return this;
    }
    /// <summary>
    /// Executes the pipeline
    /// </summary>
    /// <param name="context">the context of the pipeline</param>
    /// <returns></returns>
    public async Task ExecuteAsync(TContext context)
    {
        var firstNode = _steps.First;
        if (firstNode == null) return;
        await ExecuteNodeAsync(firstNode, context);
    }

    private async Task ExecuteNodeAsync(LinkedListNode<Func<TContext, Func<TContext, Task>, Task>> node, TContext context)
    {
        var step = node.Value;
        if (node.Next != null)
            await step(context, async (ctx) => await ExecuteNodeAsync(node.Next, ctx));
        else
            await step(context, async (ctx) => await Task.CompletedTask);
    }
}
