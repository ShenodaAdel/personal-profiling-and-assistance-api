using BusinessLogic.Services.ContactUsService.Dtos;
using BusinessLogic.Services.Emails;
using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services.ContactUsService
{
    public class ContactUsService : IContactUsService
    {
        private readonly MyDbContext _context;
        private readonly IEmailServices _emailServices;

        public ContactUsService(MyDbContext context , IEmailServices emailService)
        {
            _context = context;
            _emailServices = emailService;
        }

        public async Task<string> AddContactUsAsync(AddContactUsDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return "الاسم مطلوب";

                if (string.IsNullOrWhiteSpace(dto.Email))
                    return "البريد الإلكتروني مطلوب";

                if (string.IsNullOrWhiteSpace(dto.Problem))
                    return "وصف المشكلة مطلوب";

                bool emailExists = await _context.Users.AnyAsync(c => c.Email == dto.Email);
                if (!emailExists)
                    return "هذا البريد الإلكتروني غير مسجل . يرجى استخدام بريد إلكتروني آخر.";


                var contactUs = new ContactUs
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Date = DateTime.UtcNow,
                    Problem = dto.Problem,
                    IsRead = 0
                };
                await _context.Contactus.AddAsync(contactUs);
                await _context.SaveChangesAsync();
                return "تم ارسال مشكلتك بنجاح شكرا علي تواصلكم معانا";

            }
            catch (Exception ex)
            {
                return "حدث خطأ أثناء إرسال مشكلتك. يرجى المحاولة مرة أخرى.";
            }
        }

        public async Task<List<ContactUs>> GetAllContactUsAsync()
        {
            try
            {
                var unreadContacts = await _context.Contactus 
                    .Where(c => c.IsRead == 0)  
                    .OrderByDescending(c => c.Date)  // Most recent first  
                    .ToListAsync();

                var unreadCount = await _context.Contactus 
                    .CountAsync(c => c.IsRead == 0); 

                return unreadContacts; 
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving contact us messages", ex);
            }
        }

        public async Task<string> DeleteContactUsAsync(int id)
        {
            try
            {
                var contact = await _context.Contactus.FindAsync(id);
                if (contact == null)
                {
                    return "لم يتم العثور علي المشكلة التي تريد ان تحذفها";
                }

                _context.Contactus.Remove(contact);
                await _context.SaveChangesAsync();
                return "تم الحذف بنجاح";
            }
            catch (Exception ex)
            {
                return "حدث خطأ أثناء عملية الحذف. يرجى المحاولة مرة أخرى.";
            }
        }

        public async Task<int> GetUnreadContactUsCountAsync()
        {
            try
            {
                int unreadCount = await _context.Contactus
                    .CountAsync(c => c.IsRead == 0);

                return unreadCount;
            }
            catch (Exception)
            {
                throw new Exception("Error counting unread contact us messages");
            }
        }

        public async Task<string> MarkContactUsAsReadAsync(int id)
        {
            try
            {
                var contactUs = await _context.Contactus.FirstOrDefaultAsync(c => c.Id == id);

                if (contactUs == null)
                    return "لم يتم العثور على المشكلة.";

                contactUs.IsRead = 1;
                await _context.SaveChangesAsync();

                return "تمت قراءة الرسالة بنجاح.";
            }
            catch (Exception)
            {
                return "حدث خطأ أثناء تحديث حالة الرسالة.";
            }
        }

        public async Task<string> AnswerContactUsAsync(int id, string adminAnswer)
        {
            try
            {
                var contactUs = await _context.Contactus.FirstOrDefaultAsync(c => c.Id == id);

                if (contactUs == null)
                    return "لم يتم العثور على الرسالة.";

                if (string.IsNullOrWhiteSpace(adminAnswer))
                    return "الإجابة لا يمكن أن تكون فارغة.";

                // Prepare email
                string subject = "رد على استفسارك";
                string body = $"مرحباً {contactUs.Name},\n\n{adminAnswer}\n\nشكراً لتواصلك معنا.";

                // Send email
                await _emailServices.SendEmailAsync(contactUs.Email, subject, body);

                // Optionally, mark it as read after answering
                contactUs.IsRead = 1;
                await _context.SaveChangesAsync();

                return "تم إرسال الرد بنجاح إلى البريد الإلكتروني.";
            }
            catch (Exception)
            {
                return "حدث خطأ أثناء إرسال الرد.";
            }
        }

    }
}
