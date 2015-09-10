using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skypebot.Services.authorization
{
    public class AuthorizationService : IChatBotService
    {
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
            var _param = parameters.Split(' ');
            var _user = _param[0];
            var _priviledge = _param[1];


        }
    }
}
