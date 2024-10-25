namespace AnnaFest.Quiz
{
    public class DummyQuiz : Quiz
    {
        public override int QuizId => 500;

        public override string Title => "Del ??: ??";

        public override string QuestionText => "Vilken är ???";

        public DummyQuiz()
        {
            var questionList = new List<Question>();
            AddAnswers("Answer 1");
            AddAnswers("Answer 2");
            this.Questions = questionList.ToArray();

            void AddAnswers(params string[] answers)
            {
                questionList.Add(new Question(answers));
            }
        }
    }
}
