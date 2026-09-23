using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface IPageService
    {
        Task<PageContent> GetPageByTypeAsync(string type);
        Task<PageContent> GetPageByIdAsync(int id);
        Task<IEnumerable<PageContent>> GetAllPagesAsync();
        Task UpdatePageContentAsync(PageContent pageContent);
        Task<ContactInfo> GetContactInfoAsync();
        Task UpdateContactInfoAsync(ContactInfo contactInfo);
        Task SaveContactQueryAsync(ContactQuery query);
        Task<IEnumerable<ContactQuery>> GetAllContactQueriesAsync();
        Task DeleteContactQueryAsync(int id);
    }
}
