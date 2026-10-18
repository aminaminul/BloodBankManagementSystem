using System.Threading.Tasks;
using BBDMS.Model.Models.Entities;

namespace BBDMS.Service.Interfaces
{
    public interface IPasswordResetService
    {
        Task<string> CreateResetTokenAsync(string email, int expiryMinutes = 30);
        Task<PasswordResetToken?> ValidateTokenAsync(string token);
        Task MarkTokenAsUsedAsync(string token);
    }
}
