namespace Ldc.Domain.Repositories;

public interface IUnitOfWork
{
    Task Commit();
}