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
//  29/05/2009      Guilherme Raimundo <guilherme.raimundo@inesc-id.pt>
//  Changed the constraint of TProperty from Property to IProperty
//

using ION.Meta;

namespace ION.Core.Events
{
	
	/// <remarks>
    /// Indicates that a Property has changed its value.
	/// </remarks>
    internal sealed class ValueChanged<TOldValue, TNewValue, TProperty> : Event, IValueChanged<TOldValue, TNewValue, TProperty>
        where TProperty : IProperty
    {
        public ValueChanged(TOldValue oldValue, TNewValue newValue, TProperty property)
        {
            this.property = property;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
		
		/// <summary>
        /// Gets the value before the change occured.
        /// </summary>
        public TOldValue OldValue
        {
            get { return this.oldValue; }
        }
        private readonly TOldValue oldValue;
		
        /// <summary>
        /// Gets the value after the change occured.
        /// </summary>
        public TNewValue NewValue
        {
            get { return this.newValue; }
        }
        private readonly TNewValue newValue;
		
		
         /// <summary>
        /// Gets the Property which value has changed.
        /// </summary>
        public TProperty Property
        {
            get { return this.property; }
        }
        private readonly TProperty property;

        #region IValueChanged Members
		
		/// <summary>
        /// Gets the value before the change occured.
        /// </summary>
        object IValueChanged.OldValue
        {
            get { return this.oldValue; }
        }
		
		/// <summary>
        /// Gets the value after the change occured.
        /// </summary>
        object IValueChanged.NewValue
        {
            get { return this.newValue; }
        }
		
		/// <summary>
        /// Gets the Property which value has changed.
        /// </summary>
        IProperty IValueChanged.Property
        {
            get { return this.property; }
        }

        #endregion
    }
}
