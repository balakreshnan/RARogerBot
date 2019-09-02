using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AdaptiveCards;
using System.Text;

namespace rocksecbot.Bots
{
    

    /// <summary>
    /// This bot demonstrates how to use Microsoft Translator.
    /// </summary>
    /// <remarks>
    /// More information can be found <see href="https://docs.microsoft.com/en-us/azure/cognitive-services/translator/translator-info-overview">here</see>
    /// </remarks>
    public class MultiLingualBot : ActivityHandler
    {
        private const string WelcomeText = @"This bot will introduce you to translation middleware. Say 'hi' to get started.";

        private const string EnglishEnglish = "en";
        private const string EnglishSpanish = "es";
        private const string SpanishEnglish = "in";
        private const string SpanishSpanish = "it";

        private BotState _conversationState;
        private BotState _userState1;
        private readonly UserState _userState;
        private readonly IStatePropertyAccessor<string> _languagePreference;

        public static Boolean islanguageset = false;
        public static Boolean isfirst = false;
        public static String prevQuestion = string.Empty;
        public static String prevAnswer = string.Empty;

        private ILogger<DispatchBot> _logger;
        private IBotServices _botServices;

        public MultiLingualBot(UserState userState, IBotServices botServices, ILogger<DispatchBot> logger, ConversationState conversationState)
        {
            _userState = userState ?? throw new NullReferenceException(nameof(userState));

            _languagePreference = userState.CreateProperty<string>("LanguagePreference");
            _logger = logger;
            _botServices = botServices;
            _conversationState = conversationState;
            _userState1 = userState;

        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await SendWelcomeMessageAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationStateAccessors = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationData());

            var userStateAccessors = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            var userProfile = await userStateAccessors.GetAsync(turnContext, () => new UserProfile());

            /*
            switch (!islanguageset && !isfirst)
            {
                case true:
                    // Show the user the possible options for language. If the user chooses a different language
                    // than the default, then the translation middleware will pick it up from the user state and
                    // translate messages both ways, i.e. user to bot and bot to user.
                    islanguageset = false;
                    var reply = ((Activity)turnContext.Activity).CreateReply("Choose your language:");
                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                            new CardAction() { Title = "Español", Type = ActionTypes.PostBack, Value = EnglishSpanish },
                            new CardAction() { Title = "English", Type = ActionTypes.PostBack, Value = EnglishEnglish },
                        },
                    };
                    isfirst = true;
                    islanguageset = true;
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    break;
                case false:
                    var currentLang = turnContext.Activity.Text.ToLower();
                    var lang = currentLang == EnglishEnglish || currentLang == SpanishEnglish ? EnglishEnglish : EnglishSpanish;

                    // If the user requested a language change through the suggested actions with values "es" or "en",
                    // simply change the user's language preference in the user state.
                    // The translation middleware will catch this setting and translate both ways to the user's
                    // selected language.
                    // If Spanish was selected by the user, the reply below will actually be shown in spanish to the user.
                    await _languagePreference.SetAsync(turnContext, lang, cancellationToken);
                    //var 
                    reply = ((Activity)turnContext.Activity).CreateReply($"Your current language code is: {lang}");
                    islanguageset = true;
                    isfirst = false;

                    await turnContext.SendActivityAsync(reply, cancellationToken);


                    // Save the user profile updates into the user state.
                    await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
                    break;
                default:
                    break;
            }

            switch (islanguageset && !isfirst && !(turnContext.Activity.Text.Equals("Yes") || turnContext.Activity.Text.Equals("No")))
            {
                case true:
                    // Add message details to the conversation data.
                    // Convert saved Timestamp to local DateTimeOffset, then to string for display.
                    var messageTimeOffset = (DateTimeOffset)turnContext.Activity.Timestamp;
                    var localMessageTime = messageTimeOffset.ToLocalTime();
                    conversationData.Timestamp = localMessageTime.ToString();
                    conversationData.ChannelId = turnContext.Activity.ChannelId.ToString();
                    prevQuestion = turnContext.Activity.Text;

                    await ProcessSampleQnAAsync(turnContext, cancellationToken);
                    break;
                case false:
                    await ProcessFeedbackActionsAsync(turnContext, cancellationToken);
                    break;

            }

            */



            if (IsLanguageChangeRequested(turnContext.Activity.Text) && !islanguageset)
            {
                //LanguagePreference

              
                var currentLang = turnContext.Activity.Text.ToLower();
                var lang = currentLang == EnglishEnglish || currentLang == SpanishEnglish ? EnglishEnglish : EnglishSpanish;

                // If the user requested a language change through the suggested actions with values "es" or "en",
                // simply change the user's language preference in the user state.
                // The translation middleware will catch this setting and translate both ways to the user's
                // selected language.
                // If Spanish was selected by the user, the reply below will actually be shown in spanish to the user.
                await _languagePreference.SetAsync(turnContext, lang, cancellationToken);
                var reply = ((Activity)turnContext.Activity).CreateReply($"Your current language code is: {lang}");
                islanguageset = true;
                isfirst = false;

                await turnContext.SendActivityAsync(reply, cancellationToken);
                

                // Save the user profile updates into the user state.
                await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
            }
            else
            {
                if(!islanguageset)
                {
                    // Show the user the possible options for language. If the user chooses a different language
                    // than the default, then the translation middleware will pick it up from the user state and
                    // translate messages both ways, i.e. user to bot and bot to user.
                    islanguageset = false;
                    var reply = ((Activity)turnContext.Activity).CreateReply("Choose your language:");
                    reply.SuggestedActions = new SuggestedActions()
                    {
                        Actions = new List<CardAction>()
                        {
                            new CardAction() { Title = "Español", Type = ActionTypes.PostBack, Value = EnglishSpanish },
                            new CardAction() { Title = "English", Type = ActionTypes.PostBack, Value = EnglishEnglish },
                        },
                    };
                    isfirst = true;

                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    
                }
                
            }

            if (islanguageset && !isfirst && !(turnContext.Activity.Text.Equals("Yes") || turnContext.Activity.Text.Equals("No")))
            {
                // Add message details to the conversation data.
                // Convert saved Timestamp to local DateTimeOffset, then to string for display.
                var messageTimeOffset = (DateTimeOffset)turnContext.Activity.Timestamp;
                var localMessageTime = messageTimeOffset.ToLocalTime();
                conversationData.Timestamp = localMessageTime.ToString();
                conversationData.ChannelId = turnContext.Activity.ChannelId.ToString();
                prevQuestion = turnContext.Activity.Text;

                await ProcessSampleQnAAsync(turnContext, cancellationToken);

            }


            if (turnContext.Activity.Text.Equals("Yes") || turnContext.Activity.Text.Equals("No"))
            {
                await ProcessFeedbackActionsAsync(turnContext, cancellationToken);
            }



        }

        private async Task ProcessSampleQnAAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("ProcessSampleQnAAsync");

            

            var results = await _botServices.SampleQnA.GetAnswersAsync(turnContext);
            if (results.Any())
            {
                
                prevAnswer = results.First().Answer;
                await turnContext.SendActivityAsync(MessageFactory.Text(results.First().Answer), cancellationToken);
                await SendSuggestedActionsAsync(turnContext, cancellationToken);
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not find an answer in the Q and A system for " + prevQuestion), cancellationToken);
            }
            //await turnContext.SendActivityAsync(MessageFactory.Text("How did i do?"), cancellationToken);
            

        }

        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var welcomeCard = CreateAdaptiveCardAttachment();
                    var response = CreateResponse(turnContext.Activity, welcomeCard);
                    await turnContext.SendActivityAsync(response, cancellationToken);
                    await turnContext.SendActivityAsync(turnContext.Activity.CreateReply(WelcomeText), cancellationToken);
                }
            }
        }

        // Create an attachment message response.
        private static Activity CreateResponse(IActivity activity, Attachment attachment)
        {
            var response = ((Activity)activity).CreateReply();
            response.Attachments = new List<Attachment>() { attachment };
            return response;
        }

        // Load attachment from file.
        private static Attachment CreateAdaptiveCardAttachment()
        {
            // combine path for cross platform support
            string[] paths = { ".", "Cards", "welcomeCard.json" };
            string fullPath = Path.Combine(paths);
            var adaptiveCard = File.ReadAllText(fullPath);
            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCard),
            };
        }

        private static bool IsLanguageChangeRequested(string utterance)
        {
            if (string.IsNullOrEmpty(utterance))
            {
                return false;
            }

            utterance = utterance.ToLower().Trim();
            return utterance == EnglishSpanish || utterance == EnglishEnglish
                || utterance == SpanishSpanish || utterance == SpanishEnglish;
        }

        private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {

            var reply = ((Activity)turnContext.Activity).CreateReply("Was the answer Helpful?");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                        {
                            new CardAction() {
                                Title = "Correct",
                                Type = ActionTypes.PostBack,
                                Value = "Yes",
                                Image = "https://bbiotstore.blob.core.windows.net/icons/thumbsup.jpg"
                            },
                            new CardAction() {
                                Title = "InCorrect",
                                Type = ActionTypes.PostBack,
                                Value = "No",
                                Image = "https://bbiotstore.blob.core.windows.net/icons/thumbsdown.jpg"
                            },
                        },
            };

            await turnContext.SendActivityAsync(reply, cancellationToken);



        }

        private static async Task ProcessFeedbackActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" Question Asked: " + prevQuestion);
            sb.AppendLine(" Answer Provides: " + prevAnswer);
            sb.AppendLine(" Feedback: " + turnContext.Activity.Text);
            sb.AppendLine(" Conversation ID: " + turnContext.Activity.Conversation.Id);
            sb.AppendLine(" Channel ID: " + turnContext.Activity.ChannelId.ToString());
            sb.AppendLine(" From ID: " + turnContext.Activity.From.Id);
            sb.AppendLine(" From Name: " + turnContext.Activity.From.Name);
            sb.AppendLine(" From User: " + turnContext.Activity.From.Role);
            sb.AppendLine(" Recipient ID: " + turnContext.Activity.Recipient.Id);
            sb.AppendLine(" Recipient Name: " + turnContext.Activity.Recipient.Name);
            sb.AppendLine(" Recipient Role: " + turnContext.Activity.Recipient.Role);

            sb.AppendLine(" ID: " + turnContext.Activity.Id);

            sb.AppendLine(" Local TimeStamp: " + turnContext.Activity.LocalTimestamp.Value.ToString());
            sb.AppendLine(" Locale: " + turnContext.Activity.Locale);
            sb.AppendLine(" Service URL: " + turnContext.Activity.ServiceUrl.ToString());
            sb.AppendLine(" Type: " + turnContext.Activity.Type.ToString());
            sb.AppendLine(" Channel Data : " + turnContext.Activity.ChannelData.ToString());

            //var reply = MessageFactory.Text("Thank yous for your reponse: " + turnContext.Activity.Text + " " + turnContext.Activity.Conversation.Id.ToString());
            var reply = MessageFactory.Text(sb.ToString());
           
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

    }
}
