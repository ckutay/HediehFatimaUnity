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
using System;
using System.Collections;
using System.Collections.Generic;
using ION.Meta.Events;
using ION.Meta.Requests;

namespace ION.Meta
{
    public sealed partial class Simulation : IElementCollection
    {
        private Dictionary<ulong, Element> elements;

        private void SetupElementCollection()
        {
            // setup the event handlers collection
            this.elements = new Dictionary<ulong, Element>();

            // setup the request handler for adding/removing event handlers to/from the collection
            this.SetRequestHandler(new GenericRequestHandler<AddElement, RemoveElement>(this.UpdateElementCollection, 1));
        }

        private void UpdateElementCollection(IEnumerable<AddElement> adds, IEnumerable<RemoveElement> removes)
        {
            // process requests for removing elements
            foreach (RemoveElement request in removes)
            {
                this.RemoveElement(request.Element);
            }

            // process requests for adding elements
            foreach (AddElement request in adds)
            {
                this.AddElement(request.Element);
            }
        }

        internal void AddElement(Element element)
        {
            if (element.Simulation == null)
            {
                // add the element to the simulation
                element.Simulation = this;
                this.elements.Add(element.UID, element);

                // raise an ElementAddedToSimulation<> event
                Type eventType = typeof(ElementAddedToSimulation<>);
                Type[] typeArguments = { element.Type };
                Type[] parameters = { element.Type, this.Type };
                object[] arguments = { element, this };
                Event e = Event.New(eventType, typeArguments, parameters, arguments);
                this.Raise(e);
                element.Raise(e);
            }
        }

        internal void RemoveElement(Element element)
        {
            if (element.Simulation == this)
            {
                // remove the element from the simulation
                element.Simulation = null;
                this.elements.Remove(element.UID);
                this.SynchronizeRemoval(element); // verificar esta parte

                // raise an ElementRemovedFromSimulation<> event
                Type eventType = typeof(ElementRemovedFromSimulation<>);
                Type[] typeArguments = { element.Type };
                Type[] parameters = { element.Type, this.Type };
                object[] arguments = { element, this };
                Event e = Event.New(eventType, typeArguments, parameters, arguments);
                this.Raise(e);
                element.Raise(e);
            }
        }

        #region IElementCollection Members

        void IElementCollection.Add(Element element)
        {
            // check if the element is not null
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            this.Schedule(new AddElement(element));
        }

        void IElementCollection.Remove(Element element)
        {
            // check if the element is not null
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            this.Schedule(new RemoveElement(element));
        }

        Element IElementCollection.this[ulong uid]
        {
            get 
            {
                Element element = null;
                this.elements.TryGetValue(uid, out element);
                return element;
            }
        }

        IEnumerator<Element> IEnumerable<Element>.GetEnumerator()
        {
            return this.elements.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.elements.Values.GetEnumerator();
        }

        #endregion
    }
}
