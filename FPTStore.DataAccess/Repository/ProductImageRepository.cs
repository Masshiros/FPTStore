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
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ProductImage productImage)
        {
            _db.ProductImages.Update(productImage);
        }
    }
}
