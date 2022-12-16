using System.Text;
using Plumber.Core;

namespace Tests;

public class PromiseTests
{

    [Fact]
    public async Task SimplePromise()
    {
        var promise = new Promise<StringBuilder>(async sb =>
        {
            sb.Append("1");
            await Task.CompletedTask;
        });

        promise.Then(async sb =>
        {
            sb.Append("2");
            await Task.CompletedTask;
        });

        var sb = new StringBuilder();
        var state = await promise.ResolveAsync(sb);
        Assert.Equal("12", sb.ToString());
        Assert.Equal(PromiseState.Resolved, state);
    }

    [Fact]
    public async Task SimplePromiseWithFail()
    {
        var promise = new Promise<StringBuilder>(async sb =>
        {
            await Task.FromException(new Exception());
            sb.Append("1");
        },
        async sb =>
        {
            sb.Append("2");
            await Task.CompletedTask;
        });

        var sb = new StringBuilder();
        var state = await promise.ResolveAsync(sb);
        Assert.Equal("2", sb.ToString());
        Assert.Equal(PromiseState.Rejected, state);
    }

    [Fact]
    public async Task RejectedPromiseWithCatch()
    {
        var promise = new Promise<StringBuilder>(async sb =>
        {
            sb.Append("1");
            await Task.CompletedTask;
        });

        promise.Then(async sb =>
        {
            await Task.FromException(new Exception());
            sb.Append("2");
        });

        promise.Catch(async sb =>
        {
            sb.Append("3");
            await Task.CompletedTask;
        });

        var sb = new StringBuilder();
        var state = await promise.ResolveAsync(sb);
        Assert.Equal("13", sb.ToString());
        Assert.Equal(PromiseState.Rejected, state);
    }

    [Fact]
    public async Task RejectedPromiseWithoutCatch()
    {
        var promise = new Promise<StringBuilder>(async sb =>
        {
            sb.Append("1");
            await Task.CompletedTask;
        });

        promise.Then(async sb =>
        {
            await Task.FromException(new InvalidOperationException());
            sb.Append("2");
        });

        var sb = new StringBuilder();
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await promise.ResolveAsync(sb));
        Assert.Equal("1", sb.ToString());
    }

    [Fact]
    public async Task RejectedPromiseWithFail()
    {
        var promise = new Promise<StringBuilder>(async sb =>
        {
            sb.Append("1");
            await Task.CompletedTask;
        });

        promise.Then(async sb =>
        {
            await Task.FromException(new InvalidOperationException());
            sb.Append("2");
        },
        async sb =>
        {
            sb.Append("3");
            await Task.CompletedTask;
        });

        promise.Then(async sb =>
        {
            sb.Append("4");
            await Task.CompletedTask;
        });

        var sb = new StringBuilder();
        var state = await promise.ResolveAsync(sb);
        Assert.Equal("13", sb.ToString());
        Assert.Equal(PromiseState.Rejected, state);
    }

}
