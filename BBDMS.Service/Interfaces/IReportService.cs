using System;
using System.Threading.Tasks;
using BBDMS.Model.Models.ViewModels;

namespace BBDMS.Service.Interfaces
{
    // Reporting & Analytics Service Interface
    public interface IReportService
    {
        Task<AdminReportsViewModel> BuildAsync(DateTime today);
    }
}
