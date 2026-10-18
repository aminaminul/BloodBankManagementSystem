using System;
using System.Linq;
using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;
using BBDMS.Repository.Interfaces;
using BBDMS.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BBDMS.Service.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IRepository<PasswordResetToken> _tokenRepository;

        public PasswordResetService(IRepository<PasswordResetToken> tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task<string> CreateResetTokenAsync(string email, int expiryMinutes = 30)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email address cannot be empty.", nameof(email));
            }

            var token = Guid.NewGuid().ToString("N");
            var record = new PasswordResetToken
            {
                Email = email.Trim().ToLowerInvariant(),
                Token = token,
                ExpirationDate = DateTime.UtcNow.AddMinutes(expiryMinutes),
                IsUsed = false,
                CreatedDate = DateTime.UtcNow
            };

            await _tokenRepository.AddAsync(record);
            await _tokenRepository.SaveChangesAsync();
            return token;
        }

        public async Task<PasswordResetToken?> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            var normalized = token.Trim();
            return await _tokenRepository.Query()
                .Where(t => t.Token == normalized && !t.IsUsed && t.ExpirationDate > DateTime.UtcNow)
                .OrderByDescending(t => t.Id)
                .FirstOrDefaultAsync();
        }

        public async Task MarkTokenAsUsedAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;

            var normalized = token.Trim();
            var record = await _tokenRepository.Query()
                .Where(t => t.Token == normalized)
                .FirstOrDefaultAsync();

            if (record != null)
            {
                var tracked = await _tokenRepository.GetByIdAsync(record.Id);
                if (tracked != null)
                {
                    tracked.IsUsed = true;
                    _tokenRepository.Update(tracked);
                    await _tokenRepository.SaveChangesAsync();
                }
            }
        }
    }
}
