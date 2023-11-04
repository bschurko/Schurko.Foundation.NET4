
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Schurko.Foundation.Patterns
{
  public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
  {
    public Repository()
    {
    }

    public abstract void Add(TEntity entity);

    public abstract void Update(TEntity entity);

    public abstract void Delete(TEntity entity);

    public abstract IEnumerable<TEntity> GetAll();

    public abstract TEntity GetById(int id);

    public abstract bool SaveChanges();
 
  }
}
