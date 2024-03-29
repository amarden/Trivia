﻿using Client.DataObjects;
using HW02.ClientDataObjects;
using HW02.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
        private string triviaQuestionId;
        AppData localData;
        private List<ViewGameSessionQuestions> sessionQuestions = new List<ViewGameSessionQuestions>();

        List<string> answerChoices = new List<string> { "answerOne", "answerTwo", "answerThree", "answerFour" };
        List<string> highScoreLabels = new List<string>
        {
            "highScoreOne", "highScoreTwo", "highScoreThree", "highScoreFour", "highScoreFive",
            "highScoreSix", "highScoreSeven", "highScoreEight", "highScoreNine", "highScoreTen"
        };
        public MainPage()
        {
            this.InitializeComponent();
            this.localData = new AppData();
            if(this.localData.getPlayerId() != null && this.localData.getGameSession() != null)
            {
                populateSessionQuestions();
            }
            else
            {
                showStart();
            }
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
            showElement("question");
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
                playerId = localData.getPlayerId(),
                gameSessionId = localData.getGameSession(),
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
                var submitBtn = (Button)FindName("submitBtn");
                submitBtn.IsEnabled = false;
            }
        }

        private void nextQuestion()
        {
            if(this.sessionQuestions.Count == 0)
            {
                endGame();
            }
            else
            {
                var currentQuestion = this.sessionQuestions.First();
                setQuestion(currentQuestion);
            }
        }

        private async void endGame()
        {
            progressBar.IsIndeterminate = true;
            // Make the call to the hello resource asynchronously using POST verb
            JToken payload = JObject.FromObject(JToken.FromObject(new { playerId = localData.getPlayerId(), gameSessionId = localData.getGameSession() }));
            var resultJson = await MobileServiceDotNet.InvokeApiAsync("endgamesession", payload);
            if (resultJson.HasValues)
            {
                this.localData.clearGameSession();
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

                string scoreNarrative = "You got a score of " + score + " \n\n" + highScoreText;
                scoreText.Text = scoreNarrative;
                hideElement("terminateBtn");
                hideElement("question");
                hideElement("submitBtn");
                showElement("gameScoreText");
                showElement("newGameBtn");
                progressBar.IsIndeterminate = false;
            }
        }

        private void showIncorrect()
        {
            var incorrectText = (TextBlock)FindName("incorrectText");
            incorrectText.Visibility = Visibility.Visible;
            incorrectText.Visibility = Visibility.Collapsed;

            //Task.Delay(2000).ContinueWith(_ =>
            //{
            //    incorrectText.Visibility = Visibility.Collapsed;
            //});
        }

        private void showCorrect()
        {
            var correctText = (TextBlock)FindName("correctText");
            correctText.Visibility = Visibility.Visible;
            correctText.Visibility = Visibility.Collapsed;
        }

        #region Bootstrap Session
        private async void startSession(object sender, TappedRoutedEventArgs e)
        {
            hideStart();
            var numQuestions = (TextBox)FindName("numQuestionText");
            var idText = (TextBox)FindName("playerIdText");
            this.localData.setPlayerId(idText.Text);
            progressBar.IsIndeterminate = true;
            int numberOfQuestions = getQuestionNumber();
            var parameters = new Dictionary<string, string> { ["triviaQCount"] = numberOfQuestions.ToString() };
            var questions = await MobileServiceDotNet.InvokeApiAsync<List<TriviaQuestion>>("triviaquestions", HttpMethod.Get, parameters);
            var ids = questions.Select(x => new TriviaId { id = x.Id }).ToList();
            startGameSession(ids);
        }

        private int getQuestionNumber()
        {
            var textBox = (TextBox)FindName("numQuestionText");
            var num = textBox.Text;
            if (num == "")
            {
                return 10;
            }
            else
            {
                int quesNumber;
                if (Int32.TryParse(num, out quesNumber))
                {
                    return quesNumber;
                }
                else
                {
                    //Not a number
                    return 99;
                }
            }
        }

        private async void startGameSession(List<TriviaId> ids)
        {
            // Make the call to the hello resource asynchronously using POST verb
            JToken payload = JObject.FromObject(JToken.FromObject(new { playerId = localData.getPlayerId(), triviaIds = ids.ToArray() }));
            var resultJson = await MobileServiceDotNet.InvokeApiAsync("startgamesession", payload);
            if (resultJson.HasValues)
            {
                var gameSession = resultJson.Value<string>("gameSessionId");
                this.localData.setGameSession(gameSession);
                populateSessionQuestions();
            }
        }

        private async void populateSessionQuestions()
        {
            progressBar.IsIndeterminate = true;
            hideStart();
            var parameters = new Dictionary<string, string>
            {
                ["playerId"] = localData.getPlayerId(),
                ["gameSessionId"] = localData.getGameSession(),
            };

            this.sessionQuestions = await MobileServiceDotNet.InvokeApiAsync<List<ViewGameSessionQuestions>>("playerprogress", HttpMethod.Get, parameters);
            this.sessionQuestions = this.sessionQuestions.Where(x => x.proposedAnswer == "?").ToList();
            nextQuestion();
            progressBar.IsIndeterminate = false;
        }

        private void terminateGame(object sender, TappedRoutedEventArgs e)
        {
            endGame();
        }

        private void suspendGame(object sender, TappedRoutedEventArgs e)
        {
            showStart();
        }
        #endregion

        #region UI Methods
        private void showStart()
        {
            progressBar.IsIndeterminate = false;
            var scoreText = (TextBlock)FindName("gameScoreText");
            scoreText.Text = "";
            hideElement("newGameBtn");
            hideElement("gameScoreText");
            hideElement("question");
            hideElement("answerOne");
            hideElement("answerTwo");
            hideElement("answerThree");
            hideElement("answerFour");
            hideElement("submitBtn");
            hideElement("terminateBtn");
            showElement("startNewBtn");
            showElement("playerLabelText");
            showElement("playerIdText");
            showElement("numQuestionLabelText");
            showElement("numQuestionText");

        }
        private void hideStart()
        {
            showElement("question");
            showElement("submitBtn");
            showElement("terminateBtn");
            hideElement("startNewBtn");
            hideElement("playerIdText");
            hideElement("playerLabelText");
            hideElement("numQuestionLabelText");
            hideElement("numQuestionText");
        }
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
            radioBtn.IsChecked = false;
            radioBtn.Content = text;
        }

        private void answerSelected(object sender, RoutedEventArgs e)
        {
            var submitBtn = (Button)FindName("submitBtn");
            submitBtn.IsEnabled = true;
        }

        #endregion

        private void newGame(object sender, TappedRoutedEventArgs e)
        {
            showStart();
        }

        private async void loadHighScores(object sender, TappedRoutedEventArgs e)
        {
            scoreProgressBar.IsIndeterminate = true;
            var parameters = new Dictionary<string, string> { ["playerId"] = this.localData.getPlayerId() };
            var highScores = await MobileServiceDotNet.InvokeApiAsync<List<HighScore>>("highscore", HttpMethod.Get, parameters);
            for (int i=1; i <= highScores.Count; i++)
            {
                var scoreString = i + ". " + highScores[i - 1].score + " - " + highScores[i - 1].date;
                var element = (TextBlock)FindName(this.highScoreLabels[i - 1]);
                element.Text = scoreString;
            }
            scoreProgressBar.IsIndeterminate = false;
        }
    }
}
