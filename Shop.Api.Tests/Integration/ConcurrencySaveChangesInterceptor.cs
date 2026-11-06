using Microsoft.EntityFrameworkCore.Diagnostics;
using Shop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Api.Tests.Integration;

public class ConcurrencySaveChangesInterceptor : SaveChangesInterceptor
{
    private TaskCompletionSource<bool> _twoRequestsReady = CreateTaskSource();

    private int _waitingRequests;
    public bool Enabled { get; set; }

    private static TaskCompletionSource<bool> CreateTaskSource()
    {
        return new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public void Reset()
    {
        _waitingRequests = 0;
        _twoRequestsReady = CreateTaskSource();
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (!Enabled)
        {
            return result;
        }

        var hasProductChange = eventData.Context!
            .ChangeTracker
            .Entries<Product>()
            .Any(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Modified);

        if (!hasProductChange)
        {
            return result;
        }

        var count = Interlocked.Increment(ref _waitingRequests);

        if (count == 2)
        {
            _twoRequestsReady.TrySetResult(true);
        }

        await _twoRequestsReady.Task.WaitAsync(cancellationToken);

        return result;
    }
}
