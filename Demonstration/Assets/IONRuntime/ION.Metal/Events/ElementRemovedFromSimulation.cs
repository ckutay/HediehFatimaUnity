//
//	ION Framework - Meta Classes
//	Copyright(C) 2009-11 GAIPS / INESC-ID Lisboa
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
//	Authors:  Marco Vala, Guilherme Raimundo, Rui Prada, Carlos Martinho 
//
//	Revision History:
//  ---
//  19/04/2011      Marco Vala <marco.vala@inesc-id.pt>
//  Initial version.
//  ---  
//
namespace ION.Meta.Events
{
    internal sealed class ElementRemovedFromSimulation<TElement> : Event, IElementRemovedFromSimulation, IRemovedFrom<TElement, Simulation>
        where TElement : Element
    {
        private readonly TElement element;
        private readonly Simulation simulation;

        public ElementRemovedFromSimulation(TElement element, Simulation simulation)
        {
            this.element = element;
            this.simulation = simulation;
        }

        #region IElementRemovedFromSimulation Members

        Element IElementRemovedFromSimulation.Element
        {
            get { return this.element; }
        }

        Simulation IElementRemovedFromSimulation.Simulation
        {
            get { return this.simulation; }
        }

        #endregion

        #region IRemovedFrom<TElement,Simulation> Members

        TElement IRemovedFrom<TElement, Simulation>.Item
        {
            get { return this.element; }
        }

        Simulation IRemovedFrom<TElement, Simulation>.From
        {
            get { return this.simulation; }
        }

        #endregion
    }
}
