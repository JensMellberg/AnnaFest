using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AnnaFest.Pages
{
    [IgnoreAntiforgeryToken]
    public class QuizModel : QuizPageBase
    {
        public QuizModel(IWebHostEnvironment environment, IHttpContextAccessor session) : base(environment, session)
        {
        }

        public IActionResult OnGet()
        {
            if (!this.User.Exists)
            {
                return RedirectToPage("Login", new { returnTo = "Quiz" });
            }

            if (this.User.IsAdmin)
            {
                return RedirectToPage("QuizAdmin");
            }

            this.SetViewData();
            return Page();
        }

        public IActionResult OnPost(string action, int status, string answer)
        {
            if (!this.User.Exists)
            {
                return new JsonResult("");
            }

            if (action == "joinQuiz")
            {
                this.QuizManager.AddPlayer(this.User.Name);
                return new JsonResult(this.QuizManager.PlayersJson);
            } 
            else if (action == "guess")
            {
                var isCorrect = this.QuizManager.GuessAnswer(answer, this.User.Name);
                return new JsonResult(isCorrect);
            }
            else if (action == "update")
            {
                if (!this.IsQuizStarted)
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
                else if (this.QuizManager.Status == Quiz.QuizStatus.QuestionFinished)
                {
                    return new JsonResult(new { action = "questionFinished" });
                }
            }

            return new JsonResult("");
        }
    }
}
