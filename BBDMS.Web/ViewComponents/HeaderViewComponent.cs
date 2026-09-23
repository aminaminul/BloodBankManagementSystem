using System.Threading.Tasks;
using BBDMS.Model.Models.ViewModels;
using BBDMS.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BBDMS.Web.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IPageService _pageService;

        public HeaderViewComponent(IPageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var contactInfo = await _pageService.GetContactInfoAsync();
            var donorId = HttpContext.Session.GetInt32("bbdmsdid");
            var adminId = HttpContext.Session.GetInt32("adminId");

            var viewModel = new HeaderViewModel
            {
                ContactInfo = contactInfo,
                DonorId = donorId,
                AdminId = adminId
            };

            return View(viewModel);
        }
    }
}
