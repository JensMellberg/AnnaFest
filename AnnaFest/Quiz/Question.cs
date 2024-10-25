namespace AnnaFest.Quiz
{
    public class Question
    {
        public string[] AcceptedAnswers { get; set; }

        public Question(string[] acceptedAnswers)
        {
            AcceptedAnswers = acceptedAnswers;
        }
    }
}
