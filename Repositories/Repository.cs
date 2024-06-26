﻿using Microsoft.EntityFrameworkCore;
using Music_Library_Management_Application.Data;
using Music_Library_Management_Application.Repositories.Interfaces;
using System.Linq.Expressions;

namespace Music_Library_Management_Application.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MyDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(MyDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public T GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public IEnumerable<T> GetAllByUserId(string userId)
        {
            return _dbSet.Where(e => EF.Property<string>(e, "UserId") == userId).ToList();
        }

        public T GetByIdAndUserId(int id, string userId)
        {
            return _dbSet.SingleOrDefault(e => EF.Property<int>(e, "Id") == id && EF.Property<string>(e, "UserId") == userId);
        }

        public int Count()
        {
            return _dbSet.Count();
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Count(predicate);
        }
    }
}
