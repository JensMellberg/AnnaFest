namespace AnnaFest.Quiz
{
    public class MiddleShowQuiz : Quiz
    {
        public override int QuizId => 300;

        public override string Title => "Del 3: Tv-Serier 2000-tal";

        public override string QuestionText => "Vilken är tv-serien?";

        public MiddleShowQuiz()
        {
            var questionList = new List<Question>();
            AddAnswers("Pokémon", "Pokemon");
            AddAnswers("The Simpsons");
            AddAnswers("Prison break");
            AddAnswers("Desperate Housewives");
            AddAnswers("Grey's Anatomy", "Greys Anatomy");
            AddAnswers("Dexter");
            AddAnswers("Weeds");
            AddAnswers("Twin Peaks");
            AddAnswers("Criminal Minds");
            AddAnswers("Friends", "Vänner");
            AddAnswers("How I met your mother");

            this.Questions = questionList.ToArray();

            void AddAnswers(params string[] answers)
            {
                questionList.Add(new Question(answers));
            }
        }
    }
}
