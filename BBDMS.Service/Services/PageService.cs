using System.Collections.Generic;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;
using System.Linq;

namespace BBDMS.Service.Services
{
    public class PageService : IPageService
    {
        private readonly IRepository<PageContent> _pageRepository;
        private readonly IRepository<ContactInfo> _contactRepository;
        private readonly IRepository<ContactQuery> _queryRepository;

        public PageService(IRepository<PageContent> pageRepository, IRepository<ContactInfo> contactRepository, IRepository<ContactQuery> queryRepository)
        {
            _pageRepository = pageRepository;
            _contactRepository = contactRepository;
            _queryRepository = queryRepository;
        }

        public async Task<PageContent> GetPageByTypeAsync(string type)
        {
            return (await _pageRepository.FindAsync(p => p.Type == type)).FirstOrDefault();
        }

        public async Task<ContactInfo> GetContactInfoAsync()
        {
            return (await _contactRepository.GetAllAsync()).FirstOrDefault();
        }

        public async Task SaveContactQueryAsync(ContactQuery query)
        {
            await _queryRepository.AddAsync(query);
            await _queryRepository.SaveChangesAsync();
        }
    }
}
