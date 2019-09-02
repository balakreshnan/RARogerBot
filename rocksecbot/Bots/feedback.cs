using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace rocksecbot.Bots
{
    

    public class feedback : ActivityHandler
    {
        private BotState _userState;
        private BotState _conversationState;


        public feedback(ConversationState conversationState, UserState userState)
        {
            _conversationState = conversationState;
            _userState = userState;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            var conversationStateAccessors = _conversationState.CreateProperty<ConversationFlow>(nameof(ConversationFlow));
            var flow = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationFlow());

            var userStateAccessors = _userState.CreateProperty<UserProfile>(nameof(UserProfile));
            var profile = await userStateAccessors.GetAsync(turnContext, () => new UserProfile());

            await FillOutFeedBackAsync(flow, profile, turnContext);

            // Save changes.
            await _conversationState.SaveChangesAsync(turnContext);
            await _userState.SaveChangesAsync(turnContext);
        }

        private static async Task FillOutFeedBackAsync(ConversationFlow flow, UserProfile profile, ITurnContext turnContext)
        {
            string input = turnContext.Activity.Text?.Trim();
            string message;

            switch (flow.LastQuestionAsked)
            {
                case ConversationFlow.Question.Correct:
                    await turnContext.SendActivityAsync($"You selected: Answer is Correct");
                    break;
                case ConversationFlow.Question.InCorrect:
                    await turnContext.SendActivityAsync($"You selected: Answer is IN Correct");
                    break;
            }

        }

    }
}
