using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZooParser.Content
{
    internal interface IParser : IDisposable
    {
        void Parse(String Http);
    }
}