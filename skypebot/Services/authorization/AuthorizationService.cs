using skypebot.Utility;
using System.Linq;

namespace skypebot.Services.authorization
{
    public class AuthorizationService : IChatBotService
    {
        private readonly IChatBot _chatBot;
        private readonly IAuthorizationManager _authorizationManager;
        public AuthorizationService(IChatBot chatBot, IAuthorizationManager authorizationManager)
        {
            _chatBot = chatBot;
            _authorizationManager = authorizationManager;
        }
        private readonly string[] _commands = { "!addpermission", "!removepermission", "!getpermission" };

        public bool CanHandleCommand(string command)
        {
            return _commands.Contains(command);
        }

        public void HandleCommand(string fromHandle, string fromDisplayName, string command, string parameters)
        {
            if (!_authorizationManager.HasPermission(fromHandle, this.GetType().Name)) return;
            var param = parameters.Split(' ');
            if (param.Count() > 2)
            {
                _chatBot.EnqueueMessage(
                    $"{fromDisplayName}: Wrong parameters supplied. Correct usage is !<command> <user> <priviledge>");
            }
            var user = param[0];
            var permission = param[1];
            switch (command)
            {

                case "!addpermission":
                    TrySetPermission(user, permission);
                    break;
                case "!removepermission":
                    RemovePermission(user, permission);
                    break;
                default:
                    return;
            }
        }

        private void TrySetPermission(string user, string permission)
        {
            var message = _authorizationManager.AddPermission(user, permission)
                ? "Successfully added the permission."
                : "Failed to set the permission";
            _chatBot.EnqueueMessage(message);
        }

        private void RemovePermission(string user, string permission)
        {
            var message = _authorizationManager.RemovePermission(user, permission)
                ? "Successfully removed the permission"
                : "Failed to set the permission";
            _chatBot.EnqueueMessage(message);
        }
    }
}
