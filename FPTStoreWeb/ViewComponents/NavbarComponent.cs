using FPTStore.DataAccess.Repository.IRepository;
using FPTStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace FPTStoreWeb.ViewComponents
{
    public class NavbarComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public NavbarComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            IEnumerable<Category> categories = _unitOfWork.CategoryRepository.GetAll().OrderBy(u => u.DisplayOrder);
            return View(categories);
        }
    }
}
