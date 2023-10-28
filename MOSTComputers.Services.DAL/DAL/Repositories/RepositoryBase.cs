namespace MOSTComputers.Services.DAL.DAL.Repositories;

internal abstract class RepositoryBase
{
    protected readonly IRelationalDataAccess _relationalDataAccess;

    protected RepositoryBase(IRelationalDataAccess relationalDataAccess)
    {
        _relationalDataAccess = relationalDataAccess;
    }
}