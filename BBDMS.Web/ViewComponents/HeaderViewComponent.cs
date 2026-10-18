using System.Threading.Tasks;
using BBDMS.Model.Models.ViewModels;
using BBDMS.Service.Interfaces;
using BBDMS.Web.Filters;
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

            var userId = User.GetUserId();
            var donorId = User.IsInRole(BBDMS.Web.Filters.Roles.Donor) ? userId : null;
            var adminId = User.IsInRole(BBDMS.Web.Filters.Roles.Admin) ? userId : null;

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
