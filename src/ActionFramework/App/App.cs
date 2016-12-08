using System;
using System.Collections.Generic;

namespace ActionFramework.App
{
    public abstract class App
    {
        //public abstract Guid AppId { get; }
        public abstract string Description { get; }
        public abstract List<Action> Actions { get; }

        public string AppName => GetType().Name;

        //public bool RunAction(Action action)
        //{
        //    var actionLog = new ActionLog(action.ActionName);
        //    actionLog.StartRunDate = DateTime.UtcNow;

        //    string actionMessage;
        //    var success = false;
        //    try
        //    {
        //        success = action.Execute(out actionMessage);
        //    }
        //    catch (Exception e)
        //    {
        //        actionMessage = e.Message;
        //    }

        //    actionLog.Success = success;
        //    actionLog.EndRunDate = DateTime.UtcNow;
        //    actionLog.LogMessage = actionMessage;
        //    ActionLogRepository.SaveActionLog(AppName, actionLog);

        //    return success;
        //}
    }
}
