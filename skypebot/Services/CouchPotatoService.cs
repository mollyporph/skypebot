using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repostpolice
{
    class CouchPotatoService : IChatBotService
    {
        public string[] Commands { get; } = {"addmovie", "addseries", "getseries", "getmovie"};

        public CouchPotatoService(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; private set; }
        public bool CanHandleCommand(string command)
        {
            return Commands.Contains(command);
        }

        public void HandleCommand(string command,string parameters)
        {
            switch (command)
            {
                case "addmovie":
                    break;
                case "addseries":
                    break;
                case "getseries":
                    break;
                case "getmovies":
                    break;

            }
        }
    }
}
