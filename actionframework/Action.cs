using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using ActionFramework.Configuration;
using ActionFramework.Logger;
using Microsoft.CSharp.RuntimeBinder;
using Serilog;

namespace ActionFramework
{
    /// <summary>
    /// Action.
    /// </summary>
    public abstract class Action
    {
        public string ActionId => GetType().GUID.ToString();
        public string InstanceId => Guid.NewGuid().ToString();
        public string ActionName => GetType().Name;
        public string ActionVersion => GetType().Assembly.GetName().Version.ToString();
        public ILogger logger => LogService.Logger;

        public ActionConfig Config { get; set; }

        public abstract object Run(dynamic obj);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        internal void Configure(ActionConfig configuration)
        {
            try
            {
                foreach (var prop in GetType().GetProperties())
                {
                    if (configuration == null)
                        throw new Exception($"The Action: '{this.ActionName }' is not configured!");

                    var property = configuration.Properties.Find(p => p.Key.Equals(prop.Name, StringComparison.CurrentCultureIgnoreCase));

                    if (property != null)
                    {
                        var value = property.Value;
                        prop.SetValue(this, Convert.ChangeType(value.ToString(), prop.PropertyType), null);
                    }
                }

                logger.Debug(string.Format("Successfully configured action {0}", configuration.ActionName));
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Internal Action Error Action '{this.ActionName}' could not be configured'");
                throw ex;
            }
        }

        public static object GetProperty(object o, string member)
        {
            if (o == null) throw new ArgumentNullException("object");
            if (member == null) throw new ArgumentNullException("member");
            Type scope = o.GetType();
            IDynamicMetaObjectProvider provider = o as IDynamicMetaObjectProvider;
            if (provider != null)
            {
                ParameterExpression param = Expression.Parameter(typeof(object));
                DynamicMetaObject mobj = provider.GetMetaObject(param);
                GetMemberBinder binder = (GetMemberBinder)Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, member, scope, new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(0, null) });
                DynamicMetaObject ret = mobj.BindGetMember(binder);
                BlockExpression final = Expression.Block(
                    Expression.Label(CallSiteBinder.UpdateLabel),
                    ret.Expression
                );
                LambdaExpression lambda = Expression.Lambda(final, param);
                Delegate del = lambda.Compile();
                return del.DynamicInvoke(o);
            }
            else
            {
                return o.GetType().GetProperty(member, BindingFlags.Public | BindingFlags.Instance).GetValue(o, null);
            }
        }
    }
}