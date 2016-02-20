using HW02.DataObjects;
using HW02.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HW02.Controllers
{
    public class HighScoreController : ApiController
    {
        private MobileServiceContext db = new MobileServiceContext();

        [HttpGet]
        public HttpResponseMessage EndGame(string playerId)
        {
            var highScores = db.HighScores
                .Where(x => x.playerId == playerId)
                .OrderByDescending(x=>x.score)
                .Select(x=> new
                {
                    date = x.CreatedAt,
                    score = x.score
                })
                .ToList();

            if (highScores.Count == 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Your player id is not associated with any scores");
            }

            return Request.CreateResponse(HttpStatusCode.OK, highScores);
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
