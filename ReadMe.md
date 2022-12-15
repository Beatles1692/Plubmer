# What is Plumber?
Plumber is a very lightweight pipeline that allow us to add each step of a logic to it and later we can Execute these steps.
Each step can either be a `Func<TContext, Func<TContext,Task>, Task>` or an object implementing the `IPipeLineStep<TContext>` interface.

# Context
Each pipeline should have a context that will be passed to each step of the pipeline. This context can be anything. Each step can modify the context and pass it to the next step.

# Next Step
Each step will receive the next step of the pipeline and will be able to call it when it is ready. A Step can also decide to not call the next step and stop the pipeline.


# Example
```csharp
     [Fact]
    public async Task FibonacciTest()
    {
        var fibonacciStep = new Func<List<int>, Func<List<int>, Task>, Task>(async (context, next) =>
        {
            if (context.Count == 0)
                context.Add(1);
            else if (context.Count == 1)
                context.Add(1);
            else
                context.Add(context[context.Count - 1] + context[context.Count - 2]);
            await next(context);
        });

        var pipeline = new PipeLine<List<int>>();
        Enumerable.Range(0, 10).ToList().ForEach(i => pipeline.AddNextStep(fibonacciStep));
        var context = new List<int>();
        await pipeline.ExecuteAsync(context);

        Assert.Equal(new[] { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 }, context.ToArray());
    }
```
