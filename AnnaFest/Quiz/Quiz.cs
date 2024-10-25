namespace AnnaFest.Quiz
{
    public abstract class Quiz
    {
        public abstract int QuizId { get; }

        public abstract string QuestionText { get; }

        public abstract string Title { get; }

        public Question[] Questions { get; set; }

        public int NumberOfQuestions => this.Questions.Length;

        public Question CurrentQuestion => this.Questions[questionPointer];

        public bool HasMoreQuestions => this.Questions.Length > questionPointer + 1;

        public int QuestionNumber => this.questionPointer + 1;

        public bool HasStarted { get; set; }

        public void Reset()
        {
            this.HasStarted = false;
            this.questionPointer = -1;
        }

        public void MoveToNextQuestion()
        {
            this.questionPointer++;
        }

        private int questionPointer = -1;
    }
}
