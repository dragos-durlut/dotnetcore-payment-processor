using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentProcessor.Models.Entity.Repository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> GetById(long id);
        Task<TEntity> Create(TEntity entity);
        Task Update(long id, TEntity entity);
        Task Delete(long id);
    }
}
