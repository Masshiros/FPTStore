using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPTStore.DataAccess.Data;
using FPTStore.DataAccess.Repository.IRepository;
using FPTStore.Models;

namespace FPTStore.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }
        public void Update(Product productObj)
        {
            _db.Products.Update(productObj);

        }
    }
}
