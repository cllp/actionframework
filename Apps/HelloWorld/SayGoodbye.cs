﻿using System;

namespace helloworld
{
    public class SayGoodbye : ActionFramework.Action
    {
        //settings
        public string Name { get; set; }
        public string Email { get; set; }

        public override object Run(dynamic obj)
        {
            Log(new {
                SayGoodbyeId = this.GetType().GUID.ToString(),
                Properties = new {
                    Name = Name,
                    Email = Email
                },
                Data = obj.ToString()
            });

            Log("Just saying goodbye"); 

            return DateTime.Now;
        }
    }
}
