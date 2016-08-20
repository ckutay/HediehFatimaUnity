using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ION.Meta;

namespace ION.Core.Extensions
{
    public class Item : Entity
    {
        public Item(string name) : base(name)
        {
        }

        public override void OnDestroy()
        {
        }
    }
}
