using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repostpolice
{
    interface IChatBot
    {
        void RegisterService(IChatBotService service);
        void ProcessCommand(string command);
        void JoinChat(string chatname);

    }
}
