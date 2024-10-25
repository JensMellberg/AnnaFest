using System.Linq;
using System.Net;
using System.Text;

namespace AnnaFest.Quiz
{
    public class QuizManager
    {
        private const int SecondsBeforeNextQuestion = 3;
        private const int SecondsPerQuestion = 40;
        private const int CorrectAnswerScore = 100;
        private const int ScorePerSecondLeft = 3;
        private const string ScoreTextFileName = "QuizScores.txt";
        private static QuizManager instance;

        private DateTime NextQuestionDateTime = DateTime.MaxValue;

        private DateTime TimesUpDateTime = DateTime.MaxValue;

        private object lockObject = new object();

        public static QuizManager Instance => instance ?? (instance = new QuizManager());

        public QuizStatus Status
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.status;
                }
            }
        }

        private QuizStatus status;

        public (string, bool)[] AllQuizzes
        {
            get 
            {
                lock (this.lockObject)
                {
                    return this.Quizzes.Select(x => (x.Title, x.HasStarted)).ToArray();
                }
             }
        }

        public bool PlayerHasJoined(IUser user)
        {
            lock (this.lockObject)
            {
                return this.PlayerScores.ContainsKey(user.Name);
            }
        }

        public bool IsQuizStarted
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.ActiveQuiz != null;
                }
            }
        }

        public QuizManager()
        {
            lock (this.lockObject)
            {
                this.Quizzes = new Quiz[]
                {
                    new MovieQuiz(),
                    new OldShowQuiz(),
                    new MiddleShowQuiz(),
                    new NewShowQuiz()
                };

                this.UpdatePlayersJson();
            }
        }

        private Quiz ActiveQuiz { get; set; }

        private Quiz[] Quizzes { get; set; }

        private Dictionary<string, int> PlayerScores = new Dictionary<string, int>();

        private Dictionary<string, int> PlayerAnswers = new Dictionary<string, int>();

        public bool HasMoreQuestions
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.IsQuizStarted && this.ActiveQuiz.HasMoreQuestions;
                }
            }
        }

        public object QuestionJson
        {
            get
            {
                lock (this.lockObject)
                {
                    return new
                    {
                        questionNumber = this.ActiveQuiz.QuestionNumber,
                        totalQuestions = this.ActiveQuiz.NumberOfQuestions,
                        questionText = this.ActiveQuiz.QuestionText,
                        quizTitle = this.ActiveQuiz.Title,
                        timeCounter = this.TimeCounter
                    };
                }
            }
        }

        public string CorrectAnswer
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.ActiveQuiz != null ? this.ActiveQuiz.CurrentQuestion.AcceptedAnswers[0] : string.Empty;
                }
            }
        }

        public void StartQuiz(int index)
        {
            lock (this.lockObject)
            {
                this.ActiveQuiz = this.Quizzes[index];
                this.ActiveQuiz.HasStarted = true;
                this.AdvanceQuestionLocal(null);
            }
        }

        public void Reset()
        {
            lock (this.lockObject)
            {
                this.ActiveQuiz = null;
                this.NextQuestionDateTime = DateTime.MaxValue;
                this.TimesUpDateTime = DateTime.MaxValue;
                this.status = QuizStatus.NotStarted;
                this.PlayerScores = new Dictionary<string, int>();
                this.PlayerAnswers = new Dictionary<string, int>();
                this.UpdatePlayersJson();

                foreach (var quiz in this.Quizzes)
                {
                    quiz.Reset();
                }
            }
        }

        public void AdvanceQuestion(string rootPath)
        {
            lock (this.lockObject)
            {
                this.AdvanceQuestionLocal(rootPath);
            }
        }

        public static int LehvenStein(string s, string t)
        {
            if (string.IsNullOrEmpty(s))
            {
                if (string.IsNullOrEmpty(t))
                    return 0;
                return t.Length;
            }

            if (string.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            if (Math.Abs(s.Length - t.Length) > 2)
            {
                return 3;
            }

            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }

            return d[n, m];
        }

        public bool GuessAnswer(string answer, string user)
        {
            List<string> correctAnswersCopy;
            int timeLeft;
            lock (this.lockObject)
            {
                if (this.PlayerAnswers.ContainsKey(user) || !this.PlayerScores.ContainsKey(user) || this.status != QuizStatus.QuestionShown || string.IsNullOrEmpty(answer))
                {
                    return false;
                }

               timeLeft = this.TimeCounter;
               correctAnswersCopy = new List<string>(this.ActiveQuiz.CurrentQuestion.AcceptedAnswers.Select(x => ProcessString(x)));
            }

            var isCorrect = false;
            answer = ProcessString(answer);
            foreach (var s in correctAnswersCopy)
            {
                if (answer.Length <= 5) {
                    if (answer.Equals(s, StringComparison.OrdinalIgnoreCase))
                    {
                        isCorrect = true;
                        break;
                    }
                }
                else {
                    if (LehvenStein(answer, s) <= 2) {
                        isCorrect = true;
                        break;
                    }
                }
            }

            lock (this.lockObject)
            {
                if (isCorrect)
                {
                    var score = CorrectAnswerScore + timeLeft * ScorePerSecondLeft;
                    this.PlayerScores[user] = this.PlayerScores[user] + score;
                    this.PlayerAnswers.Add(user, score);
                }
                else
                {
                    this.PlayerAnswers.Add(user, 0);
                }

                this.UpdatePlayersJson();
                return isCorrect;
            }

            string ProcessString(string s)
            {
                return s.ToLower().Replace("the", "", StringComparison.OrdinalIgnoreCase).Replace(" ", "");
            }
        }

        private void AdvanceQuestionLocal(string rootPath)
        {
            this.NextQuestionDateTime = DateTime.Now.AddSeconds(SecondsBeforeNextQuestion);
            this.status = QuizStatus.WaitingForQuestion;
            this.PlayerAnswers = new Dictionary<string, int>();
            this.UpdatePlayersJson();
            if (rootPath != null)
            {
                this.UpdateTextFile(rootPath);
            }
          
        }

        public void FinishQuiz()
        {
            lock (this.lockObject)
            {
                this.ActiveQuiz = null;
                this.status = QuizStatus.NotStarted;
                this.PlayerAnswers = new Dictionary<string, int>();
                this.UpdatePlayersJson();
            }
        }

        private void UpdateTextFile(string rootPath)
        {
            var sb = new StringBuilder();
            foreach (var key in this.PlayerScores.Keys.OrderByDescending(x => this.PlayerScores[x]))
            {
                sb.AppendLine(key);
                sb.AppendLine(this.PlayerScores[key].ToString());
            }

            File.WriteAllText(Path.Combine(rootPath, ScoreTextFileName), sb.ToString());
        }

        public void AddPlayer(string name)
        {
            lock (this.lockObject)
            {
                if (!PlayerScores.ContainsKey(name))
                {
                    PlayerScores.Add(name, 0);
                    this.UpdatePlayersJson();
                }
            }
        }

        public int TimeCounter
        {
            get
            {
                lock (this.lockObject)
                {
                    DateTime deadLine;
                    if (this.NextQuestionDateTime != DateTime.MaxValue)
                    {
                        deadLine = this.NextQuestionDateTime;
                    }
                    else if (this.TimesUpDateTime != DateTime.MaxValue)
                    {
                        deadLine = this.TimesUpDateTime;
                    }
                    else
                    {
                        return 0;
                    }

                    return (int)Math.Ceiling((deadLine - DateTime.Now).TotalSeconds);
                }
            }
        }

        public bool HasPlayerAnswered(IUser user)
        {
            lock (this.lockObject)
            {
                return this.PlayerAnswers.ContainsKey(user.Name);
            }
        }

        public bool HasPlayerAnsweredCorrectly(IUser user)
        {
            lock (this.lockObject)
            {
                return this.PlayerAnswers.TryGetValue(user.Name, out var answer) && answer > 0;
            }
        }

        public void Update()
        {
            lock (this.lockObject)
            {
                if (DateTime.Now > this.NextQuestionDateTime)
                {
                    this.NextQuestionDateTime = DateTime.MaxValue;
                    if (this.ActiveQuiz.HasMoreQuestions)
                    {
                        this.ActiveQuiz.MoveToNextQuestion();
                        this.status = QuizStatus.QuestionShown;
                        this.TimesUpDateTime = DateTime.Now.AddSeconds(SecondsPerQuestion);
                    }
                }
                else if (DateTime.Now > this.TimesUpDateTime)
                {
                    this.TimesUpDateTime = DateTime.MaxValue;
                    this.status = QuizStatus.QuestionFinished;
                }
            }
        }

        private void UpdatePlayersJson()
        {
            this.PlayersJson = this.PlayerScores.Keys.OrderByDescending(x => this.PlayerScores[x]).Select(x => new {
                name = x,
                score = this.PlayerScores[x],
                increase = this.PlayerAnswers.ContainsKey(x) ? this.PlayerAnswers[x] : 0
            }).ToArray();
        }

        private object playersJson;

        public object PlayersJson
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.playersJson;
                }
            }
            private set
            {
                this.playersJson = value;
            }
        }
    }
}
