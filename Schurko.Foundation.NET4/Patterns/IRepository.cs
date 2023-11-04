
using System.Collections.Generic;


#nullable enable
namespace Schurko.Foundation.Patterns
{
  public interface IRepository<TEntity> where TEntity : class
  {
    void Add(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);

    IEnumerable<TEntity> GetAll();

    TEntity GetById(int id);

    bool SaveChanges();
  }
}
