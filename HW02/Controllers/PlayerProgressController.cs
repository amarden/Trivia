using HW02.ClientDataObjects;
using HW02.DataObjects;
using HW02.Models;
using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Web.Http.OData;
using System.Data.Entity;

namespace HW02.Controllers
{
    public class PlayerProgressController : TableController<PlayerProgress>
    {
        private MobileServiceContext db = new MobileServiceContext();

        [HttpGet]
        public List<ViewGameSessionQuestions> GetPlayerProgress(string playerId, string gameSessionId)
        {
            var gameSessionQuestions = new List<ViewGameSessionQuestions>();
            var gameProgress = db.PlayerProgresses.Where(x => x.playerId == playerId && x.gameSessionId == gameSessionId).ToList();

            foreach(var question in gameProgress)
            {
                var vgs = new ViewGameSessionQuestions();
                vgs.Id = question.triviaQuestionId;
                TriviaQuestion tq = db.TriviaQuestions.Where(x => x.Id == question.triviaQuestionId).Single();
                vgs.answerOne = tq.answerOne;
                vgs.answerTwo = tq.answerTwo;
                vgs.answerThree = tq.answerThree;
                vgs.answerFour = tq.answerFour;
                vgs.proposedAnswer = question.proposedAnswer;
                vgs.questionText = tq.questionText;
                gameSessionQuestions.Add(vgs);
            }

            return gameSessionQuestions;
        }

        [HttpPatch]
        public AnswerEvaluation PatchPlayerProgress(UserAnswerOfTriviaQuestion patch)
        {
            var playerQuestion = db.PlayerProgresses
                .Where(x => x.playerId == patch.playerId && x.gameSessionId.Equals(patch.gameSessionId) && 
                            x.triviaQuestionId == patch.id)
                .Single();
            var triviaQuestion = db.TriviaQuestions.Where(x => x.Id == patch.id).SingleOrDefault();
            if (playerQuestion.proposedAnswer != "?")
            {
                throw new HttpException(400, "You tried to answer a question you already answered");
            }
            playerQuestion.proposedAnswer = patch.proposedAnswer;
            playerQuestion.answerEvaluation = triviaQuestion.correctAnswer == playerQuestion.proposedAnswer ?
                "correct" :
                "incorrect";
            db.Entry(playerQuestion).State = EntityState.Modified;
            db.SaveChanges();
            return new AnswerEvaluation { answerEvaluation = playerQuestion.answerEvaluation };
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