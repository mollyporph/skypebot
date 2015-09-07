using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKYPE4COMLib;

namespace repostpolice
{
    public class ChatBot : IChatBot
    {
        private static ConcurrentBag<string> _messages;
        private IEnumerable<IChatBotService> services; 
        public void RegisterService(IChatBotService service)
        {
            throw new NotImplementedException();
        }

        public void ProcessCommand(ChatMessage msg,string command)
        {
            var commandWords = command.Split(' ').ToList();
            var actualCommand = commandWords[0];
            if (actualCommand == null) throw new ArgumentNullException(nameof(actualCommand));
            var parameters = string.Join(" ",commandWords.Remove(actualCommand));
            Task.Run(
                () =>
                    services.OrderBy(x => x.Priority)?
                        .FirstOrDefault(x => x.CanHandleCommand(command))?
                        .HandleCommand(msg.FromHandle,msg.FromDisplayName, actualCommand, parameters));
        }

        public void JoinChat(string chatname)
        {
            throw new NotImplementedException();
        }

        public void PrintMessages(Chat chat)
        {

            string currentMessage;
            var success = _messages.TryTake(out currentMessage);
            if (!success) return;
#if DEBUG
            Debug.WriteLine(currentMessage);
#else
            chat.SendMessage(currentMessage);
#endif


        }

        public static  void EnqueueMessage(string message)
        {
            _messages.Add(message);
        }
    }
}
