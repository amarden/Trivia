using HW02.ClientDataObjects;
using HW02.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MobileServiceClient MobileServiceDotNet = new MobileServiceClient("http://localhost:57484");
        private string playerId;
        private string gameSessionId;
        private string triviaQuestionId;
        private List<ViewGameSessionQuestions> sessionQuestions = new List<ViewGameSessionQuestions>();


        List<string> answerChoices = new List<string> { "answerOne", "answerTwo", "answerThree", "answerFour" };
        public MainPage()
        {
            this.playerId = "amarden";
            this.gameSessionId = "63f4625c-1f27-448a-986e-b3ca273688c2";
            this.InitializeComponent();
            //startSession();
            populateSessionQuestions();
        }

        public void setQuestion(ViewGameSessionQuestions question)
        {
            this.triviaQuestionId = question.Id;
            setQuestion(question.questionText);
            setRadioBtnText("answerOne", question.answerOne);
            setRadioBtnText("answerTwo", question.answerTwo);
            setRadioBtnText("answerThree", question.answerThree);
            setRadioBtnText("answerFour", question.answerFour);
            showElement("answerOne");
            showElement("answerTwo");
            showElement("answerThree");
            showElement("answerFour");
        }

        public async void submitQuestion(object sender, TappedRoutedEventArgs e)
        {
            progressBar.IsIndeterminate = true;
            string chosenAnswer = "";
            foreach (string answer in answerChoices)
            {
                var radioChoice = (RadioButton)FindName(answer);
                if (radioChoice.IsChecked == true)
                {
                    chosenAnswer = answer.Replace("answer","").ToLower();
                    break;
                }
            }
            var userAnswer = new UserAnswerOfTriviaQuestion
            {
                playerId = this.playerId,
                gameSessionId = this.gameSessionId,
                id = this.triviaQuestionId,
                proposedAnswer = chosenAnswer
            };
            JToken payload = JObject.FromObject(JToken.FromObject(userAnswer) );
            //// Make the call to the hello resource asynchronously using POST verb
            var resultJson = await MobileServiceDotNet.InvokeApiAsync("playerprogress", payload, new System.Net.Http.HttpMethod("PATCH"), null);

            // Verify that a result was returned
            if (resultJson.HasValues)
            {
                hideElement("answerOne");
                hideElement("answerTwo");
                hideElement("answerThree");
                hideElement("answerFour");
                var questionThatWasAnswered = sessionQuestions.Where(x => x.Id == triviaQuestionId).Single();
                sessionQuestions.Remove(questionThatWasAnswered);
                // Extract the value from the result
                var eval = resultJson.Value<string>("answerEvaluation");
                if(eval=="correct")
                {
                    showCorrect();
                }
                else if(eval=="incorrect")
                {
                    showIncorrect();
                }
                progressBar.IsIndeterminate = false;
                nextQuestion();
                var submitBtn = (Button)FindName("SubmitBtn");
                submitBtn.IsEnabled = false;
            }
        }

        private void nextQuestion()
        {
            if(this.sessionQuestions.Count == 0)
            {
                endGame();
            }
            var currentQuestion = this.sessionQuestions.First();
            setQuestion(currentQuestion);
        }

        private async void endGame()
        {
            progressBar.IsIndeterminate = true;
            // Make the call to the hello resource asynchronously using POST verb
            JToken payload = JObject.FromObject(JToken.FromObject(new { playerId = playerId, gameSessionId = this.gameSessionId }));
            var resultJson = await MobileServiceDotNet.InvokeApiAsync("endgamesession", payload);
            if (resultJson.HasValues)
            {
                string score = resultJson.Value<string>("score");
                string beat = resultJson.Value<string>("highScoreBeat");
                var scoreText = (TextBlock)FindName("gameScoreText");

                string highScoreText;
                switch(beat)
                {
                    case "1":
                        highScoreText = "You beat your top score";
                        break;
                    case "2":
                        highScoreText = "You beat your 2nd highest score";
                        break;
                    case "3":
                        highScoreText = "You beat your 3rd highest score";
                        break;
                    case "-1":
                        highScoreText = "You did not beat any of your top 10 scores";
                        break;
                    default:
                        highScoreText = "You beat your "+beat+"th highest score";
                        break;
                }

                string scoreNarrative = "You got a score of " + score + " " + highScoreText;
                scoreText.Text = scoreNarrative;
                showElement("gameScoreText");
            }
            progressBar.IsIndeterminate = false;
        }

        private void showIncorrect()
        {
            var incorrectText = (TextBlock)FindName("incorrectText");
            incorrectText.Visibility = Visibility.Visible;
            Task.Delay(2000).ContinueWith(_ =>
            {
                incorrectText.Visibility = Visibility.Collapsed;
            });
        }

        private void showCorrect()
        {
            var correctText = (TextBlock)FindName("correctText");
            correctText.Visibility = Visibility.Visible;
            correctText.Visibility = Visibility.Collapsed;
        }

        #region Bootstrap Session
        private async void startSession()
        {
            var scoreText = (TextBlock)FindName("gameScoreText");
            scoreText.Text = "";
            hideElement("gameScoreText");
            progressBar.IsIndeterminate = true;
            int numberOfQuestions = 10;
            var parameters = new Dictionary<string, string> { ["triviaQCount"] = numberOfQuestions.ToString() };
            var questions = await MobileServiceDotNet.InvokeApiAsync<List<TriviaQuestion>>("triviaquestions", HttpMethod.Get, parameters);
            var ids = questions.Select(x => new TriviaId { id = x.Id }).ToList();
            startGameSession(ids);
        }

        private async void startGameSession(List<TriviaId> ids)
        {
            // Make the call to the hello resource asynchronously using POST verb
            JToken payload = JObject.FromObject(JToken.FromObject(new { playerId = playerId, triviaIds = ids.ToArray() }));
            var resultJson = await MobileServiceDotNet.InvokeApiAsync("startgamesession", payload);
            if (resultJson.HasValues)
            {
                this.gameSessionId = resultJson.Value<string>("gameSessionId");
            }
        }

        private async void populateSessionQuestions()
        {
            var parameters = new Dictionary<string, string>
            {
                ["playerId"] = this.playerId,
                ["gameSessionId"] = this.gameSessionId,
            };

            this.sessionQuestions = await MobileServiceDotNet.InvokeApiAsync<List<ViewGameSessionQuestions>>("playerprogress", HttpMethod.Get, parameters);
            nextQuestion();
            progressBar.IsIndeterminate = false;
        }
        #endregion

        #region UI Methods
        private void showElement(string name)
        {
            var element = (FrameworkElement)FindName(name);
            element.Visibility = Visibility.Visible;
        }

        private void hideElement(string name)
        {
            var element = (FrameworkElement)FindName(name);
            element.Visibility = Visibility.Collapsed;
        }

        private void setQuestion(string text)
        {
            var question = (TextBlock)FindName("question");
            question.Text = text;
        }

        private void setRadioBtnText(string name, string text)
        {
            var radioBtn = (RadioButton)FindName(name);
            radioBtn.Content = text;
        }

        private void answerSelected(object sender, RoutedEventArgs e)
        {
            var submitBtn = (Button)FindName("SubmitBtn");
            submitBtn.IsEnabled = true;
        }
        #endregion
    }
}
