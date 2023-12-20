using FPTStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using FPTStore.DataAccess.Repository.IRepository;
using FPTStore.Utility;
using Microsoft.AspNetCore.Authorization;
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
        * @DESC: Display details page
        * @METHOD: GET
        * @PARAM: int
        * @RETURN: ViewResult
        *
        */
        public IActionResult Details(int id)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.ProductRepository.Get(u => u.ProductId == id, includeProperties: "Category"),
                Count = 1,
                ProductId = id
            };
           

            return View(cart);
        }
        /**
        * @DESC: Add to cart on details page
        * @METHOD: POST
        * @PARAM: ShoppingCart
        * @RETURN: ViewResult
        *
        */
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            // get user id
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;
            // prevent duplicate shopping cart with same userId and productId
            ShoppingCart cartExist = _unitOfWork.ShoppingCartRepository.Get(u =>
                u.ApplicationUserId == userId && u.ProductId == shoppingCart.ProductId);
            if (cartExist != null)
            {
                // cart already exist - update the count in cart 
                cartExist.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCartRepository.Update(cartExist);
                _unitOfWork.Save();
            }
            else
            {
                // new cart - add new
                _unitOfWork.ShoppingCartRepository.Add(shoppingCart);
                _unitOfWork.Save();
                // set session
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCartRepository.GetAll(u =>
                    u.ApplicationUserId == userId).Count());
            }

            TempData["success"] = "Add to cart successfully";
          
           

            return RedirectToAction(nameof(Index));
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
