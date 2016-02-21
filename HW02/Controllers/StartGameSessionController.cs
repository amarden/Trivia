using HW02.ClientDataObjects;
using HW02.DataObjects;
using HW02.Models;
using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace HW02.Controllers
{
    public class StartGameSessionController : TableController<GameSession>
    {
        private MobileServiceContext db = new MobileServiceContext();

        [HttpPost]
        public HttpResponseMessage Post(ClientStartGameSession gameInfo)
        {
            var newGameSession = new GameSession();
            newGameSession.playerId = gameInfo.playerId;
            newGameSession.gameSessionId = Guid.NewGuid().ToString();

            if (gameInfo.triviaIds.Count() == 0 || gameInfo.triviaIds.Count() > 30)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    "You have asked for too few or too many trivia questions, must be between 1 and 30");
            }
            var idsNotFound = new List<Object>();
            var playerProgressQuestions = new List<PlayerProgress>();
            foreach (var triviaId in gameInfo.triviaIds)
            {
                var triviaQuestion = db.TriviaQuestions.Where(x => x.Id == triviaId.id).SingleOrDefault();
                if (triviaQuestion == null)
                {
                    var idNotFound = new { id = triviaId };
                    idsNotFound.Add(idNotFound);
                }
                else
                {
                    var playerProgress = new PlayerProgress();
                    playerProgress.Id = Guid.NewGuid().ToString();
                    playerProgress.triviaQuestionId = triviaId.id;
                    playerProgress.playerId = gameInfo.playerId;
                    playerProgress.gameSessionId = newGameSession.gameSessionId;
                    playerProgress.proposedAnswer = "?";
                    playerProgress.TriviaQuestion = triviaQuestion;
                    playerProgressQuestions.Add(playerProgress);
                }
            }
            if(idsNotFound.Count > 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    idsNotFound
                });
            }
            db.PlayerProgresses.AddRange(playerProgressQuestions);
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                newGameSession
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}