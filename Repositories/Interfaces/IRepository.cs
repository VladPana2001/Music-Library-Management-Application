using System.Linq.Expressions;

namespace Music_Library_Management_Application.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        public IEnumerable<T> GetAllByUserId(string userId);
        public T GetByIdAndUserId(int id, string userId);
        int Count();
        int Count(Expression<Func<T, bool>> predicate);
    }
}
