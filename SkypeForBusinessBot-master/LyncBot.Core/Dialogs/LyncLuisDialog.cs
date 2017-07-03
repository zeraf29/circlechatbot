using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyncBot.Core.Dialogs
{
    //[LuisModel("27ecf3ea-0141-44f9-91c0-303a6738c110", "fa572a05a8f74dd6a869625307fb59e4")]
    [LuisModel("93da28ed-ba7e-4a93-a6ce-e97e860f8cb4", "fa572a05a8f74dd6a869625307fb59e4")]
    [Serializable]
    public class LyncLuisDialog : LuisDialog<object>
    {
        private PresenceService _presenceService;

        public LyncLuisDialog(PresenceService presenceService)
        {
            _presenceService = presenceService;
        }
        [LuisIntent("")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            //await context.PostAsync("I'm sorry. I didn't understand you.");
            // Dont do anything. Pretend I am busy.
            context.Wait(MessageReceived);
        }

        [LuisIntent("HiGreetings")]
        public async Task HiGreetings(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            var activity = await message;
            string name = GetName(activity.From);
            await context.PostOnlyOnceAsync(Responses.HiGreetingsResponse(name), nameof(HiGreetings));
            context.Wait(MessageReceived);
        }

        [LuisIntent("채널")]
        public async Task Channel(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            
            await context.PostOnlyOnceAsync(Responses.ChannelResponse(), nameof(Channel));
            context.Wait(MessageReceived);
        }

        [LuisIntent("GoodMorningGreetings")]
        public async Task GoodMorningGreetings(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            string name = string.Empty;
            if (!context.PrivateConversationData.ContainsKey(nameof(HiGreetings)))
            {
                var activity = await message;
                name = GetName(activity.From);
            }
            await context.PostOnlyOnceAsync(Responses.GoodMorningGreetingsResponse(name), nameof(GoodMorningGreetings));
            context.Wait(MessageReceived);
        }

        [LuisIntent("Call")]
        public async Task Call(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            await context.PostOnlyOnceAsync(Responses.CallResponse(), nameof(Call));
            _presenceService.SetPresenceBusy();
            context.Wait(MessageReceived);
        }

        [LuisIntent("검색")]
        [LuisIntent("검 색")]
        [LuisIntent("Search")]
        public async Task SearchAPI(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            string searchWords = string.Empty;
            if (!context.PrivateConversationData.ContainsKey(nameof(SearchAPI)))
            {
                var activity = await message;
                //searchWords = GetName(activity.From);
                searchWords = result.Query;
                //searchWords = searchWords.Replace("검색 ", "");
                searchWords = searchWords.Replace("검색 ", "");


            }
            await context.PostOnlyOnceAsync(Responses.SearchApiResponse(searchWords), nameof(SearchAPI));
            context.Wait(MessageReceived);
        }

        private static string GetName(ChannelAccount from)
        {
            string name = string.Empty;
            if (string.IsNullOrEmpty(from.Name))
                return name;

            var res = from.Name.Split(' ');
            foreach (var item in res)
            {
                if (item.Length > 1)
                {
                    name = item;
                    break;
                }
            }
            return name;
        }
    }
}
