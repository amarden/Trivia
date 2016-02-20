using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HW02.DataObjects
{
    public class TriviaQuestion : EntityData
    {
        public string questionText { get; set; }
        public string answerOne { get; set; }
        public string answerTwo { get; set; }
        public string answerThree { get; set; }
        public string answerFour { get; set; }
        public string correctAnswer { get; set; }

    }
}