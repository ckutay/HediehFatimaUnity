using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ION.Core.Events;
using ION.Meta;
using ION.Core.Extensions.Events;


namespace ION.Core.Extensions
{
    namespace EntityProperty
    {
        public enum SetValuePolicy : byte { First, Last, Random, Unknown }
    }


    public class EntityProperty<T> : Property<T>, IEntityProperty
    {
        public Entity Parent { get; private set; }
        public string Name { get; private set; }

        public EntityProperty(Entity owner, string name, T value) 
        {
            this.RequestHandlers.Clear<SetValue>();
            this.RequestHandlers.Set<RestrictedSetValue>(this.ChangeValueChooseFirst);
            this.Parent = owner;
            this.Name = name;
            this.value = value;
            this.EventHandlers.Add<IEvent>(this.OnEvent);
        }

        public void OnEvent(IEvent evt)
        {
            this.Parent.Raise((Event)evt);
        }


        #region requests
        protected sealed class RestrictedSetValue : Request
        {
            private string visibility;
 
            public RestrictedSetValue(string visibility, T newValue) : base()
            {
                this.visibility = visibility;
                this.NewValue = newValue;
            }

            public readonly T NewValue;

            public string Visibility
            {
                get { return this.visibility; }
            }
        }
        #endregion

        #region Request Handlers

        /// <summary>
        /// Change value policy that chooses the first request set value.
        /// </summary>
        /// <param name="requests"></param>
        protected void ChangeValueChooseFirst(IEnumerable<RestrictedSetValue> requests)
        {
            this.ExecuteSetValue(requests.First());
            return;
        }

        #endregion

        protected void ExecuteSetValue(RestrictedSetValue request)
        {
            if (this.value == null && request.NewValue == null)
            {
                return;
            }

            
            Event evt = new RestrictedPropertyChange(this.Parent, request.Visibility, this, this.value, request.NewValue);

            ApplicationLogger.Instance().WriteLine("Setting an existing property value and generating the corresponding event: " + this.Parent.Name + " " + this.Name + " " + this.value);
                    
            //this.NewRestrictedPropertyChange(request.Visibility,this.value, request.NewValue);
            this.value = request.NewValue;
            this.Raise(evt);
        }

        /// <summary>
        /// Creates a new RestrictedPropertyChange Event with the appropriate types by using reflection.
        /// </summary>
        /// <returns>New ValueChanged Event.</returns>
        protected Event NewRestrictedPropertyChange(string visibility, T oldValue, T newValue)
        {
            object[] arguments = { visibility, oldValue, newValue, this };
            Type[] parameterTypes = new Type[4];
            Type[] genericTypes = new Type[3];

            parameterTypes[0] = typeof(string);

            //If the property has a reference type and is not instanciated then use base type for the OldValue type
            if (oldValue == null)
            {
                parameterTypes[1] = typeof(T);
                genericTypes[0] = typeof(T);
            }
            else
            {
                parameterTypes[1] = oldValue.GetType();
                genericTypes[0] = oldValue.GetType();
            }

            //If the property has a reference type and new value is null then use base type for the NewValue type
            if (newValue == null)
            {
                parameterTypes[2] = typeof(T);
                genericTypes[1] = typeof(T);
            }
            else
            {
                parameterTypes[2] = newValue.GetType();
                genericTypes[1] = newValue.GetType();
            }
            
            parameterTypes[3] = this.GetType();
            genericTypes[2] = this.GetType();

            return Event.New(typeof(RestrictedPropertyChange<,,>), genericTypes,parameterTypes, arguments);
        }


        public void SetRestrictedValue(string visibility, T value)
        {
            this.Schedule(new RestrictedSetValue(visibility, value));
        }
    }
}
