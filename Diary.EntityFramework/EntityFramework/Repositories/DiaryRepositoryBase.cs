using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace Diary.EntityFramework.Repositories
{
    public abstract class DiaryRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<DiaryDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected DiaryRepositoryBase(IDbContextProvider<DiaryDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class DiaryRepositoryBase<TEntity> : DiaryRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected DiaryRepositoryBase(IDbContextProvider<DiaryDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
