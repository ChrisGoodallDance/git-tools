# git-tools
CLI to execute git commands over multiple directories.

Requires the .NET 7 SDK

## Building

Build the tools, then add an alias to your bash profile. E.g.

```
alias git-tools="dotnet /git-tools/bin/Release/net7.0/git-tools.dll"
```

## Usage

```
USAGE:
    git-tools.dll [OPTIONS] <COMMAND>

EXAMPLES:
    git-tools.dll links --open
    git-tools.dll tags release
    git-tools.dll tags release --path ./path/to/git/repo
    git-tools.dll compare-tag BuildNumber.txt --tag-prefix release

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    checkout       Perform a checkout
    pull           Perform a pull
    fetch          Perform a fetch
    branch         Show the current checked out branch
    links          Open all repositories in web browser
    tags           Show latest tags on current branch
    compare-tag    Compare contents of a file (BuildNumber.txt) with a tag
    cat            Show the contents of a file
```
