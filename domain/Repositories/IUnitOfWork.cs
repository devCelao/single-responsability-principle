namespace domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<bool> CommitAsync();
}