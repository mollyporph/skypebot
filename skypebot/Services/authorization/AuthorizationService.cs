using skypebot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skypebot.Services.authorization
{
    public class AuthorizationService : IChatBotService
    {
        private IChatBot _chatBot;
        private IAuthorizationManager _authorizationManager;
        public AuthorizationService(IChatBot chatBot, IAuthorizationManager authorizationManager)
        {
            _chatBot = chatBot;
            _authorizationManager = authorizationManager;
        }
        private string[] _commands = new[] { "!addpermission", "!removepermission", "!getpermission" };
        public int Priority
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool CanHandleCommand(string command)
        {
            return _commands.Contains(command);
        }

        public void HandleCommand(string fromHandle, string fromDisplayName, string command, string parameters)
        {
            if (!_authorizationManager.HasPermission(fromHandle, this.GetType().Name))return;
            switch (command)
            {
                case "!addpermission":
                    var _param = parameters.Split(' ');
                    if (_param.Count() > 2)
                    {
                        _chatBot.EnqueueMessage($"{fromDisplayName}: Wrong parameters supplied. Correct usage is !<command> <user> <priviledge>");
                    }
                    var _user = _param[0];
                    var _priviledge = _param[1];
                    TrySetPermission(_user, _priviledge);

                    break;
                case "!removepermission":
                    break;
                case "!getpermission":
                    break;
            }


        }

        private void TrySetPermission(string _user, string _priviledge)
        {
            throw new NotImplementedException();
        }
    }
}
