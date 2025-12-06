/*
            DO WHAT THE FUCK YOU WANT TO PUBLIC LICENSE 
                    Version 2, December 2004 

 Copyright (C) 2015 LiYin <wmliyin@gmail.com> 

 Everyone is permitted to copy and distribute verbatim or modified 
 copies of this license document, and changing it is allowed as long 
 as the name is changed. 

            DO WHAT THE FUCK YOU WANT TO PUBLIC LICENSE 
   TERMS AND CONDITIONS FOR COPYING, DISTRIBUTION AND MODIFICATION 

  0. You just DO WHAT THE FUCK YOU WANT TO.
*/
using UnityEngine;
using Warudo.Core;
using Warudo.Core.Data;
using Warudo.Core.Events;
using Warudo.Core.ModUtils;
using Warudo.Core.Plugins;

/*
 * Auto Binding for Warudo Plugin
 * 
 * [Mixin]
 * [TypeIdFilter("Id-Of-Plugin")]
 * PluginProxy pluginProxy;
 * 
 * Usage:
 * pluginProxy.CallCommand<ArgsType, ReturnType>("CommandName", args);
 * 
 * If plugin not found, it will return CommandResult with status ENTITY_NOT_FOUND
 * 
 * Plugin Asset Node are all Entity. So you can use Entity extension method to call command directly:
 * entity.CallCommand<ArgsType, ReturnType>("CommandName", args);
 * 
 * [DataInput]
 * Asset asset;
 * 
 * var result = asset.CallCommand<ArgsType, ReturnType>("CommandName", args);
 * 
 */

namespace WarudoKnife.LiYin
{
    public class PluginProxy : BehavioralMixin
    {
        Plugin targetPlugin = null;
        public override void OnCreate()
        {
            var meta = GetMixinMeta();
            if (meta.TypeIdFilterAttribute != null)
            {
                var typeId = meta.TypeIdFilterAttribute.TypeId;
                Owner.Subscribe<PluginEnableEvent>((e) =>
                {
                    if (e.Plugin.GetTypeMeta().Id == typeId)
                    {
                        targetPlugin = e.Plugin;
                    }
                }, true);
                Owner.Subscribe<PluginDisableEvent>((e) =>
                {
                    if (e.Plugin.GetTypeMeta().Id == typeId)
                    {
                        targetPlugin = null;
                    }
                }, true);
                targetPlugin = Context.PluginManager.GetPlugin(typeId);
            } 
            else
            {
                Debug.LogError("PluginProxy missing [TypeIdFilter(\"Id\")] on it.");
            }
        }
        public Plugin GetTargetPlugin() => targetPlugin;
    }

    public static class  PluginProxyExtensions
    {
        public static CommandResult<TResult> CallCommand<TArgs, TResult>(this PluginProxy proxy, string commandName, TArgs args) where TArgs : class, new()
        {
            if (proxy == null)
            {
                Debug.LogError("PluginProxy missing [Mixin] on it.");
                CommandResult<TResult> commandResult = new CommandResult<TResult>();
                commandResult.Status = CommandResultStatus.EXECUTION_ERROR;
                return commandResult;
            }
            return proxy.GetTargetPlugin().CallCommand<TArgs, TResult>(commandName, args);
        }
    }

    public static class EntityExtensions
    {
        public static CommandResult<TResult> CallCommand<TArgs, TResult>(this Entity entity,string commandName, TArgs args) where TArgs : class, new()
        {
            if (entity == null)
            {
                CommandResult<TResult> commandResult = new CommandResult<TResult>();
                commandResult.Status = CommandResultStatus.ENTITY_NOT_FOUND;
                return commandResult;
            }
            CommandResult<TResult> result = Context.PluginRouter.ExecuteCommand<TArgs, TResult>(entity, commandName, args);
            return result;
        }
    }
}

