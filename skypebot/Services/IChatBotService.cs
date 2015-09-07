using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SKYPE4COMLib;

namespace repostpolice
{
    public interface IChatBotService
    {

        int Priority { get;}

        bool CanHandleCommand(string command);
        void HandleCommand(string fromHandle,string fromDisplayName, string command,string parameters);

    }
}
