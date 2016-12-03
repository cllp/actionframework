using System;

namespace ActionFramework.App
{
    public abstract class Action
    {
        //public abstract Guid ActionId { get; }
        //public abstract string AppName { get; }
        public string ActionName => GetType().Name;
        public abstract string Description { get; }
        /// <summary>
        /// Should never be called directly. Use xxxxxx instead. TODO
        /// </summary>
        /// <returns></returns>
        public abstract bool Execute(out string actionMessage);

    }
}
