﻿namespace skypebot.Services
{
    public interface IChatBotService
    {

        bool CanHandleCommand(string command);
        void HandleCommand(string fromHandle,string fromDisplayName, string command,string parameters);

    }
}
