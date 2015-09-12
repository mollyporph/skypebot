using Ninject.Modules;
using skypebot.Services;
using skypebot.Services.authorization;
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
            Bind<IChatBotService>().To<AuthorizationService>();
#if DEBUG
            Bind<IAuthorizationManager>().To<DummyAuthorizationManager>();
#else
            Bind<IAuthorizationManager>().To<AuthorizationManager>();
#endif
            Bind<IChatBot>().To<ChatBot>().InSingletonScope();
           
 
        }
    }
}
