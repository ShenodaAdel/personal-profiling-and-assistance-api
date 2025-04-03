namespace BusinessLogic.Services.Test.Dtos
{
    public class ViewDto
    {
        public string? TestName { get; set; }
        public List<QuestionDto> Questions { get; set; }
    }

    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public string Questioncontent { get; set; }
        public List<ChoiceDto> Choices { get; set; }
    }
    public class ChoiceDto
    {
        public int ChoiceId { get; set; }
        public string ChoiceContent { get; set; }
    }

}
