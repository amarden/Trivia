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
    public class PlayerProgressController : ApiController
    {
        private MobileServiceContext db = new MobileServiceContext();

        [HttpGet]
        public IQueryable<ViewGameSessionQuestions> GetPlayerProgress(string playerId, string gameSessionId)
        {
            var config = new MapperConfiguration(cfg => 
                cfg.CreateMap<PlayerProgress, ViewGameSessionQuestions>()
                .ForMember(dto => dto.questionText, opt=>opt.MapFrom(x=>x.triviaQuestion.questionText))
                .ForMember(dto => dto.answerOne, opt=>opt.MapFrom(x=>x.triviaQuestion.answerOne))
                .ForMember(dto => dto.answerTwo, opt => opt.MapFrom(x => x.triviaQuestion.answerTwo))
                .ForMember(dto => dto.answerThree, opt => opt.MapFrom(x => x.triviaQuestion.answerThree))
                .ForMember(dto => dto.answerFour, opt => opt.MapFrom(x => x.triviaQuestion.answerFour))
                );
            string playerGameSessionId = playerId + "-" + gameSessionId;
            return db.PlayerProgresses.Where(x => x.playerId == playerId && x.gameSessionId.Equals(gameSessionId))
                .ProjectTo<ViewGameSessionQuestions>(config);
        }

        [HttpPatch]
        public AnswerEvaluation PatchPlayerProgress(PlayerProgress patch)
        {
            var playerQuestion = db.PlayerProgresses
                .Where(x => x.playerId == patch.playerId && x.gameSessionId.Equals(patch.gameSessionId) && 
                            x.triviaQuestionId == patch.triviaQuestionId)
                .Single();
            if(playerQuestion.proposedAnswer != "?")
            {
                throw new HttpException(400, "You tried to answer a question you already answered");
            }
            playerQuestion.proposedAnswer = patch.proposedAnswer;
            playerQuestion.answerEvaluation = playerQuestion.triviaQuestion.correctAnswer == playerQuestion.proposedAnswer ?
                "correct" :
                "incorrect";
            db.Entry(playerQuestion).State = EntityState.Modified;
            db.SaveChangesAsync();
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