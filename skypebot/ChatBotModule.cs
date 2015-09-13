using Ninject.Modules;
using skypebot.Services;
using skypebot.Services.authorization;
using skypebot.Services.couchpotato;
using skypebot.Services.repostpolice;
using skypebot.Services.sickbeard;
using skypebot.Utility;

namespace skypebot
{
    class ChatBotModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IChatBotService>().To<CouchPotatoService>().InSingletonScope();
            Bind<IChatBotService>().To<RepostPoliceService>().InSingletonScope();
            Bind<IChatBotService>().To<AuthorizationService>().InSingletonScope();
            Bind<IChatBotService>().To<SickbeardService>().InSingletonScope();
//#if DEBUG
            //Bind<IAuthorizationManager>().To<DummyAuthorizationManager>();
//#else
            Bind<IAuthorizationManager>().To<AuthorizationManager>();
//#endif
            Bind<IChatBot>().To<ChatBot>().InSingletonScope();
           
 
        }
    }
}
