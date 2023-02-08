namespace git_tools.Commands;

public class GlobalSettings : CommandSettings
{
    [Description("The path to perform operations against")]
    [CommandOption("--path")]
    [DefaultValue("./")]
    public string Path { get; init; } = "./";
}