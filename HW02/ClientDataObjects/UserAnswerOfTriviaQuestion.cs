using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW02.ClientDataObjects
{
    public class UserAnswerOfTriviaQuestion
    {
        public string playerId { get; set; }
        public Guid gameSessionId { get; set; }
        public string id { get; set; }
        public string proposedAnswer { get; set; }
    }
}