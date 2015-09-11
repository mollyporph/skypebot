using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using skypebot.Services;
using SKYPE4COMLib;

namespace skypebot

{

    //Unstatic all the things
    public class ChatBot : IChatBot
    {
        private static readonly ConcurrentBag<string> Messages = new ConcurrentBag<string>();
        private readonly IEnumerable<IChatBotService> _services;
        private static readonly List<string> Chats = new List<string>();

        public ChatBot(IChatBotService[] services)
        {
            _services = services;
        }
      

        public void ProcessCommand(ChatMessage msg, TChatMessageStatus status)
        {
            if (TChatMessageStatus.cmsRead == status || TChatMessageStatus.cmsSending == status)
            {
                return;
            }

            if (!Chats.Contains(msg.ChatName)) return;
            var command = msg.Body;
            var commandWords = command.Split(' ').ToList();
            var actualCommand = commandWords[0];
            if (actualCommand == null) throw new ArgumentNullException(nameof(actualCommand));
            commandWords.Remove(actualCommand);
            //Consistency?
            var parameters = string.Join(" ", commandWords);

            //Fugly cleanup 
            //@microsoft : Don't stuff boolean.tostring when its not expected.......
            Task.Run(
                () =>
                    _services.OrderBy(x => x.Priority)?
                        .FirstOrDefault(x => x.CanHandleCommand(actualCommand))?
                        .HandleCommand(msg.FromHandle, msg.FromDisplayName, actualCommand, parameters));
        }


        public void JoinChat(string chatname)
        {
            Chats.Add(chatname);
        }

        public void PrintMessages(Chat chat)
        {

            string currentMessage;
            var success = Messages.TryTake(out currentMessage);
            if (!success) return;

            //Debug.WriteLine(currentMessage);

            chat.SendMessage(currentMessage);



        }

        public void EnqueueMessage(string message)
        {
            Messages.Add(message);
        }
        
    }
}
