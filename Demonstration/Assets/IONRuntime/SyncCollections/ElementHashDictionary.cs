//	ION Framework - Synchronized Collections Classes
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
//


using ION.Meta;
using ION.SyncCollections.Events.Dictionary;
using SCG = System.Collections.Generic;

namespace ION.SyncCollections
{
    /// <summary>
    /// All events raised on the elements are also raised on the ElementHashDictionary.
    /// Added / Removed / Set Events are raised on both ElementHashDictionary and elements 
    /// </summary>
    public class ElementHashDictionary<TKey,TValue> : HashDictionary<TKey,TValue>
        where TValue : Element
    {
        public ElementHashDictionary()
        {
            this.EventHandlers.Add<IAdded<TKey, TValue>>(this.onElementAdded); 
            this.EventHandlers.Add<IRemoved<TKey, TValue>>(this.onElementRemoved);
        }

        private void onElementAdded(IAdded<TKey, TValue> evt)
        {
            if (evt.Dictionary == this)
            {
                evt.Value.EventHandlers.Add<Event>(this.onElementEvent);
                evt.Value.Raise((Event)evt);
            }
        }

        private void onElementRemoved(IRemoved<TKey, TValue> evt)
        {
            if (evt.Dictionary == this)
            {
                evt.Value.EventHandlers.Remove<Event>(this.onElementEvent);
                evt.Value.Raise((Event)evt);
            }
        }

        private void onElementEvent(Event evt)
        {
            this.Raise(evt);
        }

        public virtual void OnDestroy()
        {
            base.OnDestroy();
            this.EventHandlers.Remove<IAdded<TKey, TValue>>(this.onElementAdded);
            this.EventHandlers.Remove<IRemoved<TKey, TValue>>(this.onElementRemoved);
            foreach (TValue element in this.Values)
            {
                element.EventHandlers.Remove<Event>(this.onElementEvent);
            }
        }
    }
}
