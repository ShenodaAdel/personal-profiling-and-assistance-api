using BusinessLogic.DTOs;
using BusinessLogic.Services.Emails;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services.OTP_Service
{
    public class OTPService : IOTPService
    {
        private readonly MyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailServices _emailServices;

        public OTPService(MyDbContext context, UserManager<ApplicationUser> userManager , IEmailServices emailServices)
        {
            _context = context;
            _userManager = userManager;
            _emailServices = emailServices;
        }

        public async Task<ResultDto> SendOtpToEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ResultDto { Success = false, ErrorMessage = "User not found." };

            // Generate OTP
            var code = new Random().Next(100000, 999999).ToString();

            var otp = new OtpCode
            {
                UserId = user.Id,
                Code = code,
                ExpirationTime = DateTime.UtcNow.AddMinutes(10)
            };

            _context.OtpCodes.Add(otp);
            await _context.SaveChangesAsync();

            await _emailServices.SendEmailAsync(email, "Your OTP Code", $"Your OTP code is: {code}");

            return new ResultDto { Success = true, ErrorMessage = "OTP sent to your email." };
        }

        public async Task<ResultDto> VerifyOtpAndResetPasswordAsync(string email, string code, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ResultDto { Success = false, ErrorMessage = "User not found." };

            var otp = await _context.OtpCodes
                .Where(o => o.UserId == user.Id && o.Code == code && !o.IsUsed && o.ExpirationTime > DateTime.UtcNow)
                .OrderByDescending(o => o.ExpirationTime)
                .FirstOrDefaultAsync();

            if (otp == null)
                return new ResultDto { Success = false, ErrorMessage = "Invalid or expired OTP." };

            // Reset password
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (!result.Succeeded)
                return new ResultDto { Success = false, ErrorMessage = string.Join(", ", result.Errors.Select(e => e.Description)) };

            otp.IsUsed = true;
            await _context.SaveChangesAsync();

            return new ResultDto { Success = true, ErrorMessage = "Password reset successfully." };
        }
    }
}
