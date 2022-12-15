using Plumber.Core;

namespace Tests.TestObjects;
public class LogMessageStep : IPipelineStep<MessageContext>
{
    private readonly string _message;

    public LogMessageStep(string message)
    {
        _message = message;
    }
    public async Task ExecuteAsync(MessageContext context, Func<MessageContext, Task> next)
    {
        context.Count++;
        if (context.Count > context.MaxCount)
            await Task.CompletedTask;
        else
        {
            context.Log(_message);
            Console.WriteLine(_message);
            await next(context);
        }

    }
}