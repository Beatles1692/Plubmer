using System.Diagnostics;
using Plumber.Core;
using Tests.TestObjects;

namespace Tests;

public class PipeLineTests
{
    [Fact]
    public async Task PipeLineMainFlow()
    {
        string nl = Environment.NewLine;
        string EXPECTED_RESULT = $"Hello{nl}Odd{nl}World!{nl}Even{nl}and Hello{nl}Odd{nl}World! for the second time{nl}Even{nl}and yet Hello{nl}Odd{nl}World! for the third time{nl}Even{nl}and again Hello{nl}Odd{nl}World! for the fourth time{nl}Even{nl}";
        var pipeLine = new PipeLine<MessageContext>();

        var OddOrEven = new Func<MessageContext, Func<MessageContext, Task>, Task>(async (context, next) =>
        {
            if (context.Count % 2 == 0)
                context.Log("Even");
            else
                context.Log("Odd");
            await next(context);
        });

        pipeLine.AddNextStep(new AuthenticationStep())
                .AddNextStep(new LogMessageStep("Hello"))
                .AddNextStep(OddOrEven)
                .AddNextStep(new LogMessageStep("World!"))
                .AddNextStep(OddOrEven)
                .AddNextStep(new LogMessageStep("and Hello"))
                .AddNextStep(OddOrEven)
                .AddNextStep(new LogMessageStep("World! for the second time"))
                .AddNextStep(OddOrEven)
                .AddNextStep(new LogMessageStep("and yet Hello"))
                .AddNextStep(OddOrEven)
                .AddNextStep(new LogMessageStep("World! for the third time"))
                .AddNextStep(OddOrEven)
                .AddNextStep(new LogMessageStep("and again Hello"))
                .AddNextStep(OddOrEven)
                .AddNextStep(new LogMessageStep("World! for the fourth time"))
                .AddNextStep(OddOrEven);

        var context = new MessageContext { MaxCount = 10, UserName = "User is OK" };
        await pipeLine.ExecuteAsync(context);
        Assert.Equal(EXPECTED_RESULT, context.GetLog());
    }

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
}