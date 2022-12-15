namespace Plumber.Core;
public class PipeLine<TContext>
{

    private readonly LinkedList<Func<TContext, Func<TContext, Task>, Task>> _steps = new LinkedList<Func<TContext, Func<TContext, Task>, Task>>();

    public PipeLine<TContext> AddNextStep(IPipelineStep<TContext> step) => AddNextStep((context, next) => step.ExecuteAsync(context, next));

    public PipeLine<TContext> AddNextStep(Func<TContext, Func<TContext, Task>, Task> step)
    {
        _steps.AddLast(step);
        return this;
    }
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
