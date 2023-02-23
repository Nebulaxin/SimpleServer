using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleServer
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServerMethodAttribute : Attribute
    {
        public string Name { get; private set; }

        public ServerMethodAttribute(string name)
        {
            Name = name;
        }
    }
}