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
            var objFromDb = _db.Products.FirstOrDefault(u => u.ProductId == productObj.ProductId);
            if (objFromDb != null)
            {
                objFromDb.ProductTitle = productObj.ProductTitle;
                objFromDb.ISBN = productObj.ISBN;
                objFromDb.Price = productObj.Price;
                objFromDb.Price50 = productObj.Price50;
                objFromDb.ListPrice = productObj.ListPrice;
                objFromDb.Price100 = productObj.Price100;
                objFromDb.ProductDescription = productObj.ProductDescription;
                objFromDb.CategoryId = productObj.CategoryId;
                objFromDb.ProductAuthor = productObj.ProductAuthor;
                objFromDb.ProductImages = productObj.ProductImages;
                /*  if (productObj.ImageUrl != null)
                  {
                      objFromDb.ImageUrl = productObj.ImageUrl;
                  }*/
            }

        }
    }
}
