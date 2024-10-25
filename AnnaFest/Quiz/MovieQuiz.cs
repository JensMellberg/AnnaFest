namespace AnnaFest.Quiz
{
    public class MovieQuiz : Quiz
    {
        public override int QuizId => 100;

        public override string Title => "Del 1: Filmer";

        public override string QuestionText => "Vilken är filmen?";

        public MovieQuiz()
        {
            var questionList = new List<Question>();
            AddAnswers("Lord of the rings", "Sagan om ringen", "Sagan om härskarringen");
            AddAnswers("Mission Impossible", "Mission: Impossible");
            AddAnswers("Pirates of the caribbean", "Pirates");
            AddAnswers("Schindler's List", "Schindlers List");
            AddAnswers("8 mile", "eight mile");
            AddAnswers("Top Gun");
            AddAnswers("The Sound of Music");
            AddAnswers("James Bond", "Agent 007");
            AddAnswers("Indiana Jones");
            AddAnswers("Jaws", "Hajen");
            AddAnswers("Star wars", "Stjärnornas krig");
            AddAnswers("Lion King", "Lejonkungen");
            AddAnswers("Harry Potter");
            AddAnswers("The Bodyguard", "Bodyguard");
            AddAnswers("Titanic");
            AddAnswers("Grease");
            AddAnswers("Saturday night fever");
            AddAnswers("Psycho");
            AddAnswers("Jurassic Park");
            AddAnswers("Terminator", "The terminator", "Dödsängeln");
            this.Questions = questionList.ToArray();

            void AddAnswers(params string[] answers)
            {
                questionList.Add(new Question(answers));
            }
        }
    }
}
