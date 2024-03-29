﻿using HW02.DataObjects;
using HW02.Models;
using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HW02.Controllers
{
    public class EndGameSessionController : TableController<GameSession>
    {
        private MobileServiceContext db = new MobileServiceContext();

        [HttpPost]
        public HttpResponseMessage EndGame(GameSession session)
        {
            List<PlayerProgress> pp = db.PlayerProgresses
                .Where(x => x.playerId == session.playerId && x.gameSessionId.Equals(session.gameSessionId)).ToList();
            GameSession gs = db.GameSessions
                .Where(x => x.playerId == session.playerId && x.gameSessionId == session.gameSessionId).Single();
            gs.state = "Done";
            if (pp.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Your player id is not associated with the game session");
            }

            int rightAnswers = pp.Where(x => x.answerEvaluation == "correct").Count();
            int wrongAnswers = pp.Where(x => x.answerEvaluation != "correct").Count();

            int score = rightAnswers > wrongAnswers ? rightAnswers - wrongAnswers : 0;
            int rank = compareScore(session.playerId, score);

            db.PlayerProgresses.RemoveRange(pp);
            db.Entry(gs).State = EntityState.Modified;
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                score = score,
                highScoreBeat = rank
            });
        }

        private int compareScore(string playerId, int score)
        {
            HighScore hs = new HighScore { playerId = playerId, score = score, Id = Guid.NewGuid().ToString() };

            int rank = -1;
            var highScores = db.HighScores
                .Where(x => x.playerId == playerId)
                .OrderByDescending(x=>x.score)
                .ToList();

            for(int i=0; i < highScores.Count; i++)
            {
                if(score > highScores[i].score)
                {
                    rank = i + 1;
                    break;
                } 
            }

            if(highScores.Count < 10 || rank != -1)
            {
                db.HighScores.Add(hs);
            }
            if(highScores.Count >= 10)
            {
                var lowestScore = highScores.Min(x => x.score);
                var highScoreToRemove = highScores.Where(x => x.score == lowestScore).First();
                db.HighScores.Remove(highScoreToRemove);
            }

            return rank;
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