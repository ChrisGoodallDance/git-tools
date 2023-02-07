var serviceCollection = new ServiceCollection();
var typeRegistrar = new TypeRegistrar(serviceCollection);

serviceCollection.AddSingleton<IGitService, GitService>();
serviceCollection.AddSingleton<IProcessService, ProcessService>();
serviceCollection.AddSingleton<IFileService, FileService>();

var commandApp = new CommandApp(typeRegistrar);
commandApp.Configure(config =>
{
    config.AddCommand<TagsCommand>("tags")
        .WithExample(new [] {"tags", "release" })
        .WithExample(new [] { "tags", "release", "--path", "./path/to/git/repo" });
    
    config.AddCommand<CatCommand>("cat");
    config.AddCommand<FetchCommand>("fetch");
    config.AddCommand<PullCommand>("pull");
    config.AddCommand<LinksCommand>("links")
        .WithExample(new [] { "links", "--open" });
    
    config.AddCommand<CheckoutCommand>("checkout");
    config.AddCommand<CurrentBranchCommand>("current-branch");
    config.AddCommand<CompareFileAndTagCommand>("compare-tag")
        .WithExample(new [] { "compare-tag", "BuildNumber.txt", "--tag-prefix", "release" });
});

return await commandApp.RunAsync(args);