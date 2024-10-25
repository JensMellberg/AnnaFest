namespace AnnaFest.Quiz
{
    public class OldShowQuiz : Quiz
    {
        public override int QuizId => 200;

        public override string Title => "Del 2: Tv-serier 80,90-tal";

        public override string QuestionText => "Vilken är tv-serien?";

        public OldShowQuiz()
        {
            var questionList = new List<Question>();
            AddAnswers("Glamour", "The bold and beautiful");
            AddAnswers("Hem till gården", "Emmerdale");
            AddAnswers("Law and order", "I lagens namn");
            AddAnswers("Buffy och vampyrerna", "Buffy & vampyrerna", "Buffy the vampire slayer", "Buffy");
            AddAnswers("Doctor who", "Dr. who", "Dr who");
            AddAnswers("Arkiv X", "The X-files", "The X files");
            AddAnswers("Fresh prince of Bel Air", "Fresh Prince i Bel Air", "Fresh prince");
            AddAnswers("Stargate SG-1", "Stargate");
            AddAnswers("Våra värsta år", "Married with children");
            AddAnswers("Cityakuten", "ER");

            this.Questions = questionList.ToArray();

            void AddAnswers(params string[] answers)
            {
                questionList.Add(new Question(answers));
            }
        }
    }
}
