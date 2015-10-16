using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace skypebot.Services.diceservice
{
    public class DiceService : IChatBotService
    {
        Random r;
        IChatBot _chatBot;
        Regex diceRegex = new Regex(@"^(\d{1}\s)?(?:d|t)(\d{1,3})$", RegexOptions.Compiled);
        public DiceService(IChatBot chatBot)
        {
             r = new Random();
            _chatBot = chatBot;
        }
        public bool CanHandleCommand(string command)
        {
            return new[] { "!dice", "!roll" }.Contains(command);
        }

        public void HandleCommand(string fromHandle, string fromDisplayName, string command, string parameters)
        {
            var value = 0;
           
            var match = diceRegex.Match(parameters);
            string message = "";
            if (match.Success)
            {
                if(!string.IsNullOrWhiteSpace(match.Groups[1].Value))
                {
                    var count = 0;
                    int.TryParse(match.Groups[1].Value, out count);
                    for(int i = 0;i<count;i++)
                    {
                        var val = 0;
                        int.TryParse(match.Groups[2].Value, out val);
                        value += rollDie(val);
                    }

                    message = $"{fromDisplayName} rolled {value.ToString()} on {count}x {match.Groups[0].Value.Split(' ')[1].ToUpper()}.";
                }
                else
                {
                    var val = 0;
                    int.TryParse(match.Groups[2].Value, out val);
                    value += rollDie(val);
                    message = $"{fromDisplayName} rolled {value.ToString()} on a {match.Groups[0].Value.ToUpper()}.";
                }
                _chatBot.EnqueueMessage(message);
            }


        }
        public int rollDie(int max)
        {
            max = Math.Max(1, max);
            return r.Next(1, max);
        }
    }
}
