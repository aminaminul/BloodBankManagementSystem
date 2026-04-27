using System.Threading.Tasks;
using BBDMS.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BBDMS.Web.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IPageService _pageService;

        public FooterViewComponent(IPageService pageService)
        {
            _pageService = pageService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var contactInfo = await _pageService.GetContactInfoAsync();
            return View(contactInfo);
        }
    }
}
