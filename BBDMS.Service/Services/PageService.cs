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
            return (await _pageRepository.FindAsync(p => p.Type == type)).FirstOrDefault()!;
        }

        public async Task<PageContent> GetPageByIdAsync(int id)
        {
            return await _pageRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<PageContent>> GetAllPagesAsync()
        {
            return await _pageRepository.GetAllAsync();
        }

        public async Task UpdatePageContentAsync(PageContent pageContent)
        {
            _pageRepository.Update(pageContent);
            await _pageRepository.SaveChangesAsync();
        }

        public async Task<ContactInfo> GetContactInfoAsync()
        {
            var info = (await _contactRepository.GetAllAsync()).FirstOrDefault();
            return info ?? new ContactInfo
            {
                Address = "Central Emergency Support Tower, Medical District",
                EmailId = "emergency@bbdms-support.org",
                ContactNo = "01700000000"
            };
        }

        public async Task UpdateContactInfoAsync(ContactInfo contactInfo)
        {
            var existing = (await _contactRepository.GetAllAsync()).FirstOrDefault();
            if (existing != null)
            {
                existing.Address = contactInfo.Address;
                existing.EmailId = contactInfo.EmailId;
                existing.ContactNo = contactInfo.ContactNo;
                _contactRepository.Update(existing);
            }
            else
            {
                await _contactRepository.AddAsync(contactInfo);
            }
            await _contactRepository.SaveChangesAsync();
        }

        public async Task SaveContactQueryAsync(ContactQuery query)
        {
            query.PostingDate = System.DateTime.Now;
            query.Status = 1;
            await _queryRepository.AddAsync(query);
            await _queryRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ContactQuery>> GetAllContactQueriesAsync()
        {
            var queries = await _queryRepository.GetAllAsync();
            return queries.OrderByDescending(q => q.PostingDate).ToList();
        }

        public async Task DeleteContactQueryAsync(int id)
        {
            var query = await _queryRepository.GetByIdAsync(id);
            if (query != null)
            {
                _queryRepository.Remove(query);
                await _queryRepository.SaveChangesAsync();
            }
        }
    }
}
