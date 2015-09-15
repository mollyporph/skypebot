using skypebot.Services;
using SKYPE4COMLib;
using System;

namespace skypebot
{
    public interface IChatBot
    {
        void ProcessCommand(ChatMessage msg, TChatMessageStatus status);
        void JoinChat(string chatname);
        void PrintMessages(Chat chat);
        void EnqueueMessage(string message);
        void RegisterGlobalClockHandler(Action handler);
        void UnregisterGlobalClockHandler(Action handler);

    }
}
