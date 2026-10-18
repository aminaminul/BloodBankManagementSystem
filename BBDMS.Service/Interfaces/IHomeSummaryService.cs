using System.Threading.Tasks;
using BBDMS.Model.Models.ViewModels;

namespace BBDMS.Service.Interfaces
{
    public interface IHomeSummaryService
    {
        Task<HomeIndexViewModel> GetSummaryAsync();
    }
}
