using Plumber.Core;

namespace Tests.TestObjects;
public class AuthenticationStep : IPipelineStep<MessageContext>
{
    public async Task ExecuteAsync(MessageContext context, Func<MessageContext, Task> next)
    {
        if (string.IsNullOrWhiteSpace(context.UserName) || context.UserName.Contains("Test"))
        {
            context.Response = " User is not defined or contains Test ";
            await Task.CompletedTask;
        }
        else
            await next(context);
    }
}