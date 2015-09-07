using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKYPE4COMLib;

namespace repostpolice
{
    interface IChatBot
    {
        void RegisterService(IChatBotService service);
        void ProcessCommand(ChatMessage msg,string command);
        void JoinChat(string chatname);
        void PrintMessages(Chat chat);

    }
}
