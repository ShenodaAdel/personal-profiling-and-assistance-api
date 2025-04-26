using BusinessLogic.Services.ContactUsService;
using BusinessLogic.Services.ContactUsService.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Personal_Profiling_And_Assistance.Controllers.Contactus
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsService _contactUsService;
        public ContactUsController(IContactUsService contactUsService)
        {
            _contactUsService = contactUsService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddContactUs([FromBody] AddContactUsDto dto)
        {
            var result = await _contactUsService.AddContactUsAsync(dto);
            return Ok(result);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllContactUs()
        {
            var result = await _contactUsService.GetAllContactUsAsync();
            return Ok(result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteContactUs(int id)
        {
            var result = await _contactUsService.DeleteContactUsAsync(id);
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadContactUsCount()
        {
            var count = await _contactUsService.GetUnreadContactUsCountAsync();
            return Ok(new { unreadCount = count });
        }

        [HttpPut("mark-as-read/{id}")]
        public async Task<IActionResult> MarkContactUsAsRead(int id)
        {
            var result = await _contactUsService.MarkContactUsAsReadAsync(id);
            return Ok(result);
        }

        [HttpPost("answer/{id}")]
        public async Task<IActionResult> AnswerContactUs(int id, [FromBody] string adminAnswer)
        {
            var result = await _contactUsService.AnswerContactUsAsync(id, adminAnswer);
            return Ok(result);
        }
    }
}
