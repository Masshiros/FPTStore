using FPTStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FPTStore.DataAccess.Repository.IRepository;
using X.PagedList;

namespace FPTStoreWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        /**
         * @DESC: Display home page . When there is param catId - type id, filter product by category
         * @METHOD: GET
         * @PARAM: int,int
         * @RETURN: ViewResult
         *
         */
        public IActionResult Index(int? catId, int? page)
        {
            int pageSize = 8;
            int pageNumber = page == null|| page < 0 ? 1 : page.Value;


            IEnumerable<Product> objProductList;
            if (catId != null)
            {
                objProductList = _unitOfWork.ProductRepository.Filter(u => u.CategoryId == catId);
            }
            else
            {
                objProductList = _unitOfWork.ProductRepository.GetAll(includeProperties: "Category").ToList();
            }

            PagedList<Product> productList = new PagedList<Product>(objProductList, pageNumber,pageSize);
            return View(productList);
        }
        /**
        * @DESC: Display home page
        * @METHOD: GET
        * @PARAM
        * @RETURN: ViewResult
        *
        */
        public IActionResult Details(int id)
        {
            Product productObj = _unitOfWork.ProductRepository.Get(u => u.ProductId == id,includeProperties: "Category");

            return View(productObj);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
