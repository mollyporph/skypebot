using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using skypebot.Services;
using SKYPE4COMLib;
using System.Timers;

namespace skypebot

{

    //Unstatic all the things
    public class ChatBot : IChatBot
    {
        private static readonly ConcurrentBag<string> Messages = new ConcurrentBag<string>();
        private static readonly List<string> Chats = new List<string>();
        private Timer _timer;
        private List<Action> handlers;

        [Inject]
        public IEnumerable<IChatBotService> _services { private get; set; } 
        public ChatBot()
        {
            _timer = new Timer(60000);
            _timer.Elapsed += _timer_Elapsed;

            _timer.Start();
            handlers = new List<Action>();
        }

      

        public void ProcessCommand(ChatMessage msg, TChatMessageStatus status)
        {
            if (TChatMessageStatus.cmsRead == status || TChatMessageStatus.cmsSending == status)
            {
                return;
            }
#if DEBUG
#else
            if (!Chats.Contains(msg.ChatName)) return;
#endif
            var command = msg.Body;
            var commandWords = command.Split(' ').ToList();
            var actualCommand = commandWords[0];
            if (actualCommand == null) throw new ArgumentNullException(nameof(actualCommand));
            commandWords.Remove(actualCommand);
            //Consistency?
            var parameters = string.Join(" ", commandWords);

            Task.Run(
                () =>
                    _services
                        .Where(x => x.CanHandleCommand(actualCommand)).ToList().ForEach(x =>
                        x.HandleCommand(msg.FromHandle, msg.FromDisplayName, actualCommand, parameters)));
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

        public void RegisterGlobalClockHandler(Action handler)
        {
            handlers.Add(handler);
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            handlers.AsParallel().ForAll(x => x()); 
        }

        public void UnregisterGlobalClockHandler(Action handler)
        {
            handlers.Remove(handler);
        }
    }
}
