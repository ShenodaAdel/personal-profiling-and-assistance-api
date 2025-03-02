namespace BusinessLogic.Helpers
{
    public class Jwt
    {
        public string? SecretKey { get; set; }

        public string? Issuer { get; set; }

        public string? Audience { get; set; }
        public string? DurationInDays { get; set; }
    }
}
