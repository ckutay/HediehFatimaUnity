using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ION.Core.Extensions
{
    public interface IEntityAction : IAction
    {
        Entity Parent { get; }
        string Name { get; }
    }
}
