using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ION.Meta;

namespace ION.Core.Extensions
{
    
    public interface IPropertyAdded : IEvent
    {
        IEntityProperty Property { get; }
        Entity Entity { get; }
        string Visibility { get; }
		string ToXml();
    }

   

    public class PropertyAdded : Event, IPropertyAdded
    {
        private readonly IEntityProperty property;
        private readonly Entity entity;
        private readonly string visibility;

        public PropertyAdded(string visibility, IEntityProperty property, Entity entity)
        {
            this.property = property;
            this.entity = entity;
            this.visibility = visibility;
        }

        public IEntityProperty Property
        {
            get { return this.property; }
        }

        public Entity Entity
        {
            get { return this.entity; }
        }

        public string Visibility
        {
            get { return this.visibility; }
        }
		
		public string ToXml()
		{
			string xml = "<Property>";
            
            xml+= "<Visibility>" + this.visibility + "</Visibility>";
            xml+= "<Entity>" + this.entity.Name + "</Entity>";
            xml+= "<Name>" + this.property.Name + "</Name>";
            xml+= "<Value>" + this.property.Value + "</Value>";
            xml+= "</Property>";

            return xml;
		}
    }
}
