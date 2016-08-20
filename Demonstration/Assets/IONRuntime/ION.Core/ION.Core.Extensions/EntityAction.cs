using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ION.Meta;

namespace ION.Core.Extensions
{
    public class EntityAction<T> : Action<T>, IEntityAction
    {
        public Entity Parent { get; private set; }
        public string Name { get; private set; }

        public EntityAction(Entity owner, string name)
            : base()
        {
            this.Parent = owner;
            this.Name = name;
            this.EventHandlers.Add<IEvent>(this.OnEvent);
        }

        public void OnEvent(IEvent evt)
        {
            this.Parent.Raise((Event)evt);
        }
    }
}
