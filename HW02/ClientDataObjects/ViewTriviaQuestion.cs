using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HW02.ClientDataObjects
{
    public class ViewTriviaQuestion
    {
        public string questionText { get; set; }
        public string answerOne { get; set; }
        public string answerTwo { get; set; }
        public string answerThree { get; set; }
        public string answerFour { get; set; }
        public string correctAnswer { get; set; }
    }
}