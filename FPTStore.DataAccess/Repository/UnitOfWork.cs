using FPTStore.DataAccess.Data;
using FPTStore.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPTStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository CategoryRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            CategoryRepository = new CategoryRepository(_db);
            ProductRepository = new ProductRepository(_db);

        }
      

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
