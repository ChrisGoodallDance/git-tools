namespace git_tools.Settings;

public class GlobalSettings : CommandSettings
{
    [Description("The path to perform operations against")]
    [CommandOption("--path")]
    [DefaultValue("./")]
    public string Path { get; init; } = "./";
}