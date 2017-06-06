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
    [LuisModel("90ff3c2e-6afe-41b0-ae10-21b4000abc65", "249e15ad49a44eb7816c5b32bfe8b3ca")]
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

        [LuisIntent("날씨")]
        public async Task Weather(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            string zoneCode = string.Empty;
            foreach (EntityRecommendation e in result.Entities)
            {
                if (e.Entity.Contains("서 울"))
                    zoneCode = "1100000000";
                else if (e.Entity.Contains("부 산"))
                    zoneCode = "2600000000";
                else if (e.Entity.Contains("대 구"))
                    zoneCode = "2700000000";
                else if (e.Entity.Contains("인 천"))
                    zoneCode = "2800000000";
                else if (e.Entity.Contains("광 주"))
                    zoneCode = "2900000000";
                else if (e.Entity.Contains("대 전"))
                    zoneCode = "3000000000";
                else if (e.Entity.Contains("울 산"))
                    zoneCode = "3100000000";
                else if (e.Entity.Contains("세 종"))
                    zoneCode = "3611000000";
                else if (e.Entity.Contains("경 기"))
                    zoneCode = "4100000000";
                else if (e.Entity.Contains("강 원"))
                    zoneCode = "4200000000";
                else if (e.Entity.Contains("충 남"))
                    zoneCode = "4400000000";
                else if (e.Entity.Contains("전 북"))
                    zoneCode = "4500000000";
                else if (e.Entity.Contains("전 남"))
                    zoneCode = "4600000000";
                else if (e.Entity.Contains("경 북"))
                    zoneCode = "4700000000";
                else if (e.Entity.Contains("경 남"))
                    zoneCode = "4800000000";
                else if (e.Entity.Contains("제 주"))
                    zoneCode = "5000000000";

                await context.PostAsync(Responses.WeatherResponse(zoneCode), nameof(Weather));

                context.Wait(MessageReceived);
            }
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
