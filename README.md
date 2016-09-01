# skypebot
community automation through skype
[test](#test)
Builds
[![Build status](https://ci.appveyor.com/api/projects/status/rtvaha08iq25o1wc/branch/development?svg=true&passingText=Development%20-%20OK)](https://ci.appveyor.com/project/mollyporph/skypebot/branch/development)
[![Build status](https://ci.appveyor.com/api/projects/status/rtvaha08iq25o1wc/branch/master?svg=true&passingText=Production%20-%20OK)](https://ci.appveyor.com/project/mollyporph/skypebot/branch/master)

This is a modular/service approach applied to a skype chatbot.

if you want to develop your own integration or service all you have to do is implement IChatbotService and add your service to the ninjectmodule in ChatbotModule.

All chatbotservices implement IChatbotService

```csharp
public interface IChatBotService
    {
        bool CanHandleCommand(string command);
        void HandleCommand(string fromHandle,string fromDisplayName, string command,string parameters);
    }
```
You also need to add an IChatBot variable and inject it through your constructor if you want to send messages.

When your chatbotservice is done, add it as a module in chatbotmodule.cs
````csharp

  public override void Load()
        {
            Bind<IChatBotService>().To<MyAwesomeService>().InSingletonScope();
            ...
        }
````

Your CanHandleCommand should probably either use a list of commands or in case of a logger return true for all non-commands

````csharp

//A typical command-oriented service
public bool CanHandleCommand(string command)
    {
        return new List<string>(){ "!command1", "!command2" }.Contains(command);
    }
````

````csharp

//Might not want to capture the commands if you're doing a logger-type service
public bool CanHandleCommand(string command)
    {
       return !command.StartsWith("!");
    }
````

There is also a persisted user-database if you need it, which containts skype-handles and permissions. By default all permissions are rebuilt in the database through reflection (the permission beeing the typename of the service)

I've also built a small function to apply all permissions to my own handle, feel free to change this (In the future I'll add module isolation and build a config-file for admin-account so we don't keep that in code :)

To implement authorization in your HandleCommand use the following snippet:

````csharp
  public void HandleCommand(string fromHandle, string fromDisplayName, string command, string parameters)
        {
          if (!_authorizationManager.HasPermission(fromHandle, this.GetType().Name)) return;
          ....
        }
````
You also need to edit the JoinChat chatname to whichever chat you want to use. The easiest way to get the chatname of a chat is to put a breakpoint in processcommand, then look at the msg-parameter and get the chatname.



#Test
