using AnnaFest.Quiz;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnnaFest.Pages
{
    public class QuizPageBase : BasePage
	{
		public QuizManager QuizManager { get; private set; }

		public (string, bool)[] AllQuizzes => this.QuizManager.AllQuizzes;

		public bool PlayerHasJoined => this.QuizManager.PlayerHasJoined(this.User);

		public bool HasPlayerAnswered => this.QuizManager.HasPlayerAnswered(this.User);

        public bool HasPlayerAnsweredCorrectly => this.QuizManager.HasPlayerAnsweredCorrectly(this.User);

		public string CorrectAnswer => this.QuizManager.CorrectAnswer;

        public QuizPageBase(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor) : base(environment, httpContextAccessor)
		{
			this.QuizManager = QuizManager.Instance;
			this.QuizManager.Update();
			this.ScoresJson = JsonUtils.GetJavascriptText("scoresJson", this.QuizManager.PlayersJson);
			this.TimeCounter = JsonUtils.GetJavascriptText("timeCounter", this.QuizManager.TimeCounter);
            this.QuestionJson = this.QuizManager.Status == QuizStatus.QuestionShown ? 
				JsonUtils.GetJavascriptText("questionInfo", this.QuizManager.QuestionJson)
				: "";
        }

		public bool IsQuizStarted => this.QuizManager.IsQuizStarted;

		public string ScoresJson { get; set; }

        public string TimeCounter { get; set; }

		public string QuestionJson { get; set; }
    }
}