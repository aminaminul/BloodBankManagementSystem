using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface IPageService
    {
        Task<PageContent> GetPageByTypeAsync(string type);
        Task<ContactInfo> GetContactInfoAsync();
        Task SaveContactQueryAsync(ContactQuery query);
    }
}
