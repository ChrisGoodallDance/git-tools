namespace git_tools.Commands;

[Description("Perform a checkout")]
internal sealed class CheckoutCommand : PerformCommand<CheckoutCommand.Settings>
{
    public static void Register(IConfigurator config)
    {
        config.AddCommand<CheckoutCommand>("checkout");
    }
    
    internal sealed class Settings : GlobalSettings
    {
        [Description("Branch name")]
        [CommandArgument(0, "[branch]")]
        [DefaultValue("")]
        public string Branch { get; init; } = "";
    }
    
    public CheckoutCommand(IGitService gitService)
        : base(settings => "checkout " + settings.Branch, gitService)
    {
    }
}