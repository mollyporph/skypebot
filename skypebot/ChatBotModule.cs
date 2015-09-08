using Ninject.Modules;
using skypebot.Services;
using skypebot.Services.couchpotato;
using skypebot.Services.repostpolice;
using skypebot.Utility;

namespace skypebot
{
    class ChatBotModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IChatBotService>().To<CouchPotatoService>();
            Bind<IChatBotService>().To<RepostPoliceService>();
            Bind<IAuthorizationManager>().To<AuthorizationManager>();
        }
    }
}
