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

namespace HW02.Controllers
{
    public class TriviaQuestionsController : TableController<TriviaQuestion>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<TriviaQuestion>(context, Request);
        }

        [HttpGet]
        public IQueryable<TriviaQuestion> GetAllTriviaQuestions()
        {
            return Query();
        }

        [HttpGet]
        public SingleResult<TriviaQuestion> GetTriviaQuestion(string id)
        {
            return Lookup(id);
        }

        [HttpGet]
        public IQueryable<TriviaQuestion> GetRandomSetOfTriviaQuestions(int triviaQCount)
        {
            return Query().OrderBy(x => Guid.NewGuid()).Take(triviaQCount);
        }

    }
}