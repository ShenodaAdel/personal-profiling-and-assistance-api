using BusinessLogic.Services.ContactUsService.Dtos;
using Data.Models;

namespace BusinessLogic.Services.ContactUsService
{
    public interface IContactUsService
    {
        Task<string> AddContactUsAsync(AddContactUsDto dto);
        Task<List<ContactUs>> GetAllContactUsAsync();
        Task<string> DeleteContactUsAsync(int id);
        Task<int> GetUnreadContactUsCountAsync();
        Task<string> MarkContactUsAsReadAsync(int id);
        Task<string> AnswerContactUsAsync(int id, string adminAnswer);
    }
}
