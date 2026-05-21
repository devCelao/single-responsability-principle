using domain.Entities;
using domain.Events;
using domain.Repositories;
using infrastructure.Context;

namespace infrastructure.Database;

public class UnitOfWork(AppDbContext context, IDomainEventDispatcher dispatcher) : IUnitOfWork
{
    private readonly AppDbContext _context = context;
    private readonly IDomainEventDispatcher _dispatcher = dispatcher;
    public async Task<bool> CommitAsync()
    {
        var entities = _context.ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var events = entities.SelectMany(e => e.DomainEvents).ToList();
        var result = await _context.SaveChangesAsync() > 0;

        if (result)
        {
            await _dispatcher.DispatchAsync(events);
            entities.ForEach(e => e.ClearDomainEvents());
        }
        return result;
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
