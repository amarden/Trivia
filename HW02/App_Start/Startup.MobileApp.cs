using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using HW02.DataObjects;
using HW02.Models;
using Owin;

namespace HW02
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new MobileServiceInitializer());

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    // This middleware is intended to be used locally for debugging. By default, HostName will
                    // only have a value when running in an App Service application.
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }

            app.UseWebApi(config);
        }
    }

    public class MobileServiceInitializer : CreateDatabaseIfNotExists<MobileServiceContext>
    {
        protected override void Seed(MobileServiceContext context)
        {
            List<TriviaQuestion> triviaQuestions = new List<TriviaQuestion>
            {
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "What nationality was Chopin?",
                    answerOne = "Polish",
                    answerTwo = "British",
                    answerThree = "Russian",
                    answerFour = "Solvenian",
                    correctAnswer = "one"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "Who said, \"I think, therefore I am\"?",
                    answerOne = "Plato",
                    answerTwo = "Aristotle",
                    answerThree = "Descartes",
                    answerFour = "Francis Bacon",
                    correctAnswer = "three"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "Where are the Dolomites?",
                    answerOne = "Canada",
                    answerTwo = "Italy",
                    answerThree = "France",
                    answerFour = "Germany",
                    correctAnswer = "two"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "What's the capital of Ethiopia?",
                    answerOne = "Quito",
                    answerTwo = "Addis Abba",
                    answerThree = "Tegucigarpa",
                    answerFour = "Braclan",
                    correctAnswer = "two"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "How many wives did Henry the Eighth have?",
                    answerOne = "four",
                    answerTwo = "five",
                    answerThree = "six",
                    answerFour = "seven",
                    correctAnswer = "three"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "Which is the smallest ocean?",
                    answerOne = "Artic",
                    answerTwo = "Pacific",
                    answerThree = "Atlantic",
                    answerFour = "Indian",
                    correctAnswer = "one"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "What country gave Florida to the USA in 1891?",
                    answerOne = "England",
                    answerTwo = "France",
                    answerThree = "Spain",
                    answerFour = "Portugal",
                    correctAnswer = "three"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "When did the First World War start? ",
                    answerOne = "1912",
                    answerTwo = "1913",
                    answerThree = "1914",
                    answerFour = "1915",
                    correctAnswer = "three"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "How many squares are there on a chess board?",
                    answerOne = "64",
                    answerTwo = "58",
                    answerThree = "72",
                    answerFour = "81",
                    correctAnswer = "one"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "What language has the most words?",
                    answerOne = "Spanish",
                    answerTwo = "Italian",
                    answerThree = "French",
                    answerFour = "English",
                    correctAnswer = "four"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "How many colours are there in a rainbow?",
                    answerOne = "7",
                    answerTwo = "8",
                    answerThree = "6",
                    answerFour = "5",
                    correctAnswer = "one"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "How many dots are there on two dice?",
                    answerOne = "38",
                    answerTwo = "40",
                    answerThree = "42",
                    answerFour = "44",
                    correctAnswer = "three"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "What money do they use in Japan?",
                    answerOne = "dollar",
                    answerTwo = "peso",
                    answerThree = "pound",
                    answerFour = "yen",
                    correctAnswer = "four"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "What does the roman numeral C represent?",
                    answerOne = "100",
                    answerTwo = "50",
                    answerThree = "25",
                    answerFour = "500",
                    correctAnswer = "one"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "What did the crocodile swallow in Peter Pan?",
                    answerOne = "pen",
                    answerTwo = "alarm clock",
                    answerThree = "plate",
                    answerFour = "bottle",
                    correctAnswer = "two"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "Which country has the largest area: Australia, Brazil, France or India?",
                    answerOne = "Australia",
                    answerTwo = "France",
                    answerThree = "India",
                    answerFour = "Brazil",
                    correctAnswer = "four"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "What is the national religion of Japan?",
                    answerOne = "Buddhism",
                    answerTwo = "Shintoism",
                    answerThree = "Confucionism",
                    answerFour = "Hinduism",
                    correctAnswer = "two"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "In which year was the Russian Revolution?",
                    answerOne = "1917",
                    answerTwo = "1918",
                    answerThree = "1927",
                    answerFour = "1928",
                    correctAnswer = "one"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "How many bones are there in the adult human body?",
                    answerOne = "200",
                    answerTwo = "202",
                    answerThree = "204",
                    answerFour = "206",
                    correctAnswer = "four"
                },
                new TriviaQuestion
                {
                     Id = Guid.NewGuid().ToString(),
                    questionText = "Which band had a famous album cover featuring Battersea Power Station?",
                    answerOne = "Beatles",
                    answerTwo = "Led Zepplin",
                    answerThree = "Pink Floyd",
                    answerFour = "Van Halen",
                    correctAnswer = "3"
                }
            };

            foreach (TriviaQuestion question in triviaQuestions)
            {
                context.Set<TriviaQuestion>().Add(question);
            }


            base.Seed(context);
        }
    }
}

