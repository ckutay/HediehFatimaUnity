using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ION.Meta;
using ION.Core.Events;

namespace ION.Core.Extensions.Events
{
    public class RestrictedPropertyChange : Event, IRestrictedPropertyChange
    {
        private readonly IEntityProperty property;
        private readonly Entity entity;
        private readonly string visibility;
        private readonly object oldValue;
        private readonly object newValue;


        public RestrictedPropertyChange(Entity entity, string visibility, IEntityProperty p, object oldValue, object newValue)
        {
            this.visibility = visibility;
            this.entity = entity;
            this.property = p;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public IEntityProperty Property
        {
            get
            {
                return this.property;
            }
        }

        public string Visibility
        {
            get
            {
                return this.visibility;
            }
        }

        public Entity Entity
        {
            get
            {
                return this.entity;
            }
        }

        public object OldValue
        {
            get
            {
                return this.oldValue;
            }
        }

        public object NewValue
        {
            get
            {
                return this.newValue;
            }
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



        internal sealed class RestrictedPropertyChange<TOldValue, TNewValue, TProperty> : Event, IRestrictedPropertyChange<TOldValue, TNewValue, TProperty>
            where TProperty : IEntityProperty
        {
            private string visibility;

            public RestrictedPropertyChange(string visibility, TOldValue oldValue, TNewValue newValue, TProperty property) 
            {
                this.visibility = visibility;
                this.property = property;
                this.oldValue = oldValue;
                this.newValue = newValue;
            }

            public string Visibility
            { 
                get
                {
                    return this.visibility;
                }
            }

            public TOldValue OldValue
            {
                get { return this.oldValue; }
            }
            private readonly TOldValue oldValue;

            public TNewValue NewValue
            {
                get { return this.newValue; }
            }
            private readonly TNewValue newValue;

            public TProperty Property
            {
                get { return this.property; }
            }
            private readonly TProperty property;

            #region IValueChanged Members

            object IValueChanged.OldValue
            {
                get { return this.oldValue; }
            }

            object IValueChanged.NewValue
            {
                get { return this.newValue; }
            }

            IProperty IValueChanged.Property
            {
                get { return this.property; }
            }

            #endregion
        }
}
