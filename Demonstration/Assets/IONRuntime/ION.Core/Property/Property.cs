//	ION Framework - Core Classes
//	Copyright(C) 2009 GAIPS / INESC-ID Lisboa
//
//	This library is free software; you can redistribute it and/or
//	modify it under the terms of the GNU Lesser General Public
//	License as published by the Free Software Foundation; either
//	version 2.1 of the License, or (at your option) any later version.
//
//	This library is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//	Lesser General Public License for more details.
//
//	You should have received a copy of the GNU Lesser General Public
//	License along with this library; if not, write to the Free Software
//	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//	Authors:  Guilherme Raimundo, Marco Vala, Rui Prada, Carlos Martinho 
//
//	Revision History:
//  ---
//  07/04/2009      Guilherme Raimundo <guilherme.raimundo@inesc-id.pt>
//  First version.
//  ---  
//  27/04/2009      Guilherme Raimundo <guilherme.raimundo@inesc-id.pt>
//  Supply for different policies (First, Last, Random) for conflicting SetValue Requests. 
//  ---
//  04/05/2009      Guilherme Raimundo <guilherme.raimundo@inesc-id.pt>
//  Refactorized the creation of a new ValueChanged Event to be able to reuse the code on subclasses.
//  ---
//  29/05/2009      Guilherme Raimundo <guilherme.raimundo@inesc-id.pt>
//  Removed Property class since IProperty functions as the previous abstract class.
//  Created ION.Core.Property Namespace.
//  Moved SetValuePolicy enum type to ION.Core.Property Namespace.
//

using System;
using System.Linq;
using ION.Core.Events;
using ION.Core.Property;
using ION.Meta;
using ION.Meta.Events;

using System.Reflection;
//using SCG = System.Collections.Generic;
using System.Collections.Generic;

namespace ION.Core
{
    namespace Property
    {
        public enum SetValuePolicy : byte { First, Last, Random, Unknown }
    }

    /// <remarks>
    /// A Property_T is an Element that represents an attribute of a specific type.
    /// </remarks>
    public class Property<TValue> : Element, IProperty<TValue>
    {
        /// <summary>
        /// Creates a Property with the default value
        /// </summary>
        public Property()
            : this(default(TValue), SetValuePolicy.First) {}

        /// <summary>
        /// Creates a Property with a particular initial value
        /// </summary>
        public Property(TValue value)
            : this(value, SetValuePolicy.First) {}

        /// <summary>
        /// Creates a Property with a particular policy to handle conflicting SetValues requests
        /// </summary>
        public Property(SetValuePolicy policy)
            : this(default(TValue), policy) {}

        /// <summary>
        /// Creates a Property with a particular initial value and a policy to handle conflicting SetValues requests
        /// </summary>
        public Property(TValue value, SetValuePolicy policy)
        {
            this.value = value;
            this.setValuePolicy = policy;
			switch (policy)
			{
				case SetValuePolicy.First:
					this.RequestHandlers.Set<SetValue>(this.ChangeValueChooseFirst);
                    return;
                case SetValuePolicy.Last:
                    this.RequestHandlers.Set<SetValue>(this.ChangeValueChooseLast);
                    return;
                case SetValuePolicy.Random:
                    this.RequestHandlers.Set<SetValue>(this.ChangeValueChooseRandom);
                    return;
                default:
                    throw new ArgumentException("Unknown SetValuePolicy.", "policy");
            }
        }

        #region Requests

        protected sealed class SetValue : Request
        {
            public SetValue(TValue newValue)
            {
                this.NewValue = newValue;
            }

            public readonly TValue NewValue;
        }

        #endregion Requests

        #region Request Handlers
		
		
        /// <summary>
        /// Change value policy that chooses the first request set value.
        /// </summary>
        /// <param name="requests"></param>
        protected void ChangeValueChooseFirst(IEnumerable<SetValue> requests)
        {
            this.ExecuteSetValue(requests.First());
            return;
        }

        /// <summary>
        /// Change value policy that chooses the last request set value.
        /// </summary>
        /// <param name="requests"></param>
        protected void ChangeValueChooseLast(IEnumerable<SetValue> requests)
        {
            this.ExecuteSetValue(requests.Last());
            return;
        }

        /// <summary>
        /// Change value policy that chooses a random request set value.
        /// </summary>
        /// <param name="requests"></param>
        protected void ChangeValueChooseRandom(IEnumerable<SetValue> requests)
        {
			SetValue[] setValueRequests = requests.ToArray();
            this.ExecuteSetValue(setValueRequests[rand.Next(0,setValueRequests.Length-1)]);
            return;
        }

        /// <summary>
        /// Changes the value of the property and raises a ValueChanged event appropriately
        /// </summary>
        /// <param name="request"></param>
        protected void ExecuteSetValue(SetValue request)
        {
            if(this.value == null && request.NewValue == null)
            {
                return;
            }

            if (request.NewValue == null || this.value == null || !this.value.Equals(request.NewValue))
            {
                Event evt = this.NewValueChanged(this.value, request.NewValue);
                this.value = request.NewValue;
                this.Raise(evt);
            }
        }

        /// <summary>
        /// Creates a new ValueChanged Event with the appropriate types by using reflection.
        /// </summary>
        /// <returns>New ValueChanged Event.</returns>
        protected Event NewValueChanged(TValue oldValue, TValue newValue)
        {
            object[] arguments = { oldValue, newValue, this };
            Type[] parameterTypes = new Type[3];

            //If the property has a reference type and is not instanciated then use base type for the OldValue type
            if (oldValue == null)
            {
                parameterTypes[0] = typeof(TValue);
            }
            else
            {
                parameterTypes[0] = oldValue.GetType();
            }

            //If the property has a reference type and new value is null then use base type for the NewValue type
            if (newValue == null)
            {
                parameterTypes[1] = typeof(TValue);
            }
            else
            {
                parameterTypes[1] = newValue.GetType();
            }
            
            parameterTypes[2] = this.GetType();

            return Event.New(typeof(ValueChanged<,,>), parameterTypes, arguments);
        }

        #endregion

        

        /// <summary>
        /// Gets / Sets the Value of the Property.
        /// </summary>
        public TValue Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.Schedule(new SetValue(value));
            }
        }
        
        /// <summary>
        /// Direcly changes the value of the Property without regarding the synchronization cycle. 
        /// Only change this value if you know what you are doing!!!
        /// </summary>
        protected TValue value;

        public override void OnDestroy() {}

        #region IProperty Members

		
        public SetValuePolicy SetValuePolicy
        {
            get
            {
				return this.setValuePolicy;
            }
        }
		private SetValuePolicy setValuePolicy;

		
        object IProperty.Value
        {
            get { return this.value; }
        }
		
		private static Random rand = new Random();		                                        

        #endregion
    }
}