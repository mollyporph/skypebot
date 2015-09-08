using skypebot.Services;
using SKYPE4COMLib;

namespace skypebot
{
    interface IChatBot
    {
        void ProcessCommand(ChatMessage msg,string command);
        void JoinChat(string chatname);
        void PrintMessages(Chat chat);

    }
}
