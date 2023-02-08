var serviceCollection = new ServiceCollection();
var typeRegistrar = new TypeRegistrar(serviceCollection);

serviceCollection.AddSingleton<IGitService, GitService>();
serviceCollection.AddSingleton<IProcessService, ProcessService>();
serviceCollection.AddSingleton<IFileService, FileService>();

var commandApp = new CommandApp(typeRegistrar);
commandApp.Configure(config =>
{
    var methods = CommandRegistration.GetRegistrationMethods();
    Parallel.ForEach(methods, method => method.Invoke(null, new object[] { config }));
});

return await commandApp.RunAsync(args);