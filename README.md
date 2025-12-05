# Warudo Knife
A mod toolkit for Warudo.

## Features

| File | Usage | Inspired By |
|------|-------|-------------|
| PluginProxy.cs | Auto-managed binding and unbinding of Plugin references, call Command using direct method calls on Entity | [ButterKnife](https://github.com/JakeWharton/butterknife) |

## Installation
Just copy what you want into your mod project.

## Usage
### PluginProxy
Use the `PluginProxy` to call commands on other plugins without worrying about binding and unbinding the plugin reference.

If target plugin is not found, the command will return a `CommandResult` with status `ENTITY_NOT_FOUND`.

If target plugin hotloads or unloads, the `PluginProxy` will automatically update the reference.

```csharp
[AssetType(Id="WhateverMod")]
public class MyMod : Asset
{
    [Mixin]
    [TypeIdFilter("Plugin-Id-You-Want-To-Access")]
    private PluginProxy proxy;

    [Trigger]
    public void DoSomething()
    {
        CommandResult<TResult> result = proxy.CallCommand<TArgs, TResult>("CommandName", args); 
    }

    [Trigger]
    public void EquivalentDoSomething()
    {
        CommandResult<TResult> commandResult;
        var plugin = Context.PluginManager.GetPlugin("Plugin-Id-You-Want-To-Access");
        if (plugin == null) {
            commandResult = new CommandResult<TResult>();
            commandResult.Status = CommandResultStatus.ENTITY_NOT_FOUND;
        } else {
            commandResult = Context.PluginRouter.CallCommand<TArgs, TResult>(plugin, "CommandName", args);
        }
    }
}
```

### EntityExtensions (In `PluginProxy.cs`)
You can also use the `EntityExtensions` static class to call commands on plugins directly from an `Entity` instance (`Plugin` `Asset` `Node` are the subtype of `Entity`).

```csharp
[DataInput]
Asset asset;

[Trigger]
public void DoSomething()
{
    CommandResult<TResult> result = asset.CallCommand<TArgs, TResult>("CommandName", args); 
}

[Trigger]
public void EquivalentDoSomething()
{
    CommandResult<TResult> commandResult;
    if (asset == null) {
        commandResult = new CommandResult<TResult>();
        commandResult.Status = CommandResultStatus.ENTITY_NOT_FOUND;
    } else {
        commandResult = Context.PluginRouter.CallCommand<TArgs, TResult>(asset, "CommandName", args);
    }
}
```

## License
This project is licensed under the WTFPL License - see the [LICENSE](LICENSE) file for details.