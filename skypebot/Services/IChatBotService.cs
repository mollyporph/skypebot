using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repostpolice
{
    interface IChatBotService
    {

        int Priority { get;}

        bool CanHandleCommand(string command);
        void HandleCommand(string command,string parameters);

    }
}
