﻿namespace BusinessLogic.Services.ContactUsService.Dtos
{
    public class AddContactUsDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime Date { get; set; }
        public string? Problem { get; set; }
    }
}
