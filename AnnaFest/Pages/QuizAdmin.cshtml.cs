using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnnaFest.Pages
{
    [IgnoreAntiforgeryToken]
    public class QuizAdminModel : QuizPageBase
    {
        public QuizAdminModel(IWebHostEnvironment environment, IHttpContextAccessor session) : base(environment, session)
        {
        }

        public IActionResult OnGet()
        {
            if (!this.User.Exists)
            {
                return RedirectToPage("Login", new { returnTo = "Quiz" });
            }

            if (!this.User.IsAdmin)
            {
                return RedirectToPage("Index");
            }

            this.SetViewData();
            return Page();
        }

        public IActionResult OnPost(string action, int id)
        {
            if (!this.User.Exists || !this.User.IsAdmin)
            {
                return new JsonResult("");
            }

            if (action == "startQuiz")
            {
                this.QuizManager.StartQuiz(id);
                return new JsonResult("");
            }
            else if (action == "nextQuestion")
            {
                this.QuizManager.AdvanceQuestion();
                return new JsonResult("");
            }
            else if (action == "finish")
            {
                this.QuizManager.FinishQuiz();
                return new JsonResult("");
            }
            else if (action == "reset")
            {
                this.QuizManager.Reset();
            }
            else if (action == "update")
            {
                if (!this.IsQuizStarted || this.QuizManager.Status == Quiz.QuizStatus.QuestionFinished)
                {
                    return new JsonResult(new { action = "leaderBoard", scoresJson = this.QuizManager.PlayersJson });
                }
                else if (this.QuizManager.Status == Quiz.QuizStatus.WaitingForQuestion)
                {
                    return new JsonResult(new { action = "waitingForQuestion", timeCounter = this.QuizManager.TimeCounter });
                }
                else if (this.QuizManager.Status == Quiz.QuizStatus.QuestionShown)
                {
                    return new JsonResult(new { action = "questionShown", questionInfo = this.QuizManager.QuestionJson });
                }
            }

            return new JsonResult("");
        }
    }
}
