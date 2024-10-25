namespace AnnaFest.Quiz
{
    public class NewShowQuiz : Quiz
    {
        public override int QuizId => 400;

        public override string Title => "Del 4: Tv-serier 2010-tal";

        public override string QuestionText => "Vilken är tv-serien?";

        public NewShowQuiz()
        {
            var questionList = new List<Question>();
            AddAnswers("Suits");
            AddAnswers("Band of Brothers");
            AddAnswers("Outlander");
            AddAnswers("American Horror Story");
            AddAnswers("Wednesday");
            AddAnswers("Orange is the new black");
            AddAnswers("Bridgerton", "Familjen bridgerton");
            AddAnswers("Breaking Bad");
            AddAnswers("Black Mirror");
            AddAnswers("Game of Thrones");
            AddAnswers("The Walking Dead");

            this.Questions = questionList.ToArray();

            void AddAnswers(params string[] answers)
            {
                questionList.Add(new Question(answers));
            }
        }
    }
}
