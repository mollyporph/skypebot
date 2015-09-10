using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skypebot.Services.authorization
{
    public class AuthorizationService : IChatBotService
    {
        public int Priority
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool CanHandleCommand(string command)
        {
            throw new NotImplementedException();
        }

        public void HandleCommand(string fromHandle, string fromDisplayName, string command, string parameters)
        {
            throw new NotImplementedException();
        }
    }
}
