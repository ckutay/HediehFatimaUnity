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
using System.Collections.Generic;
using ION.Meta.Events;
using ION.Meta.Requests;

namespace ION.Meta
{
    public partial class Element : IEventHandlerCollection
    {
        private Dictionary<Type, HashSet<EventHandler>> eventHandlers;

        private void SetupEventHandlerCollection()
        {
            // setup the event handlers collection
            this.eventHandlers = new Dictionary<Type, HashSet<EventHandler>>();

            // setup the request handler for adding/removing event handlers
            this.SetRequestHandler(new GenericRequestHandler<AddEventHandler, RemoveEventHandler>(this.UpdateEventHandlerCollection, 1));
        }

        private void UpdateEventHandlerCollection(IEnumerable<AddEventHandler> addRequests, IEnumerable<RemoveEventHandler> removeRequests)
        {
            // process requests for removing event handlers
            foreach (RemoveEventHandler request in removeRequests)
            {
                this.RemoveEventHandler(request.Handler);
            }

            // process requests for adding event handlers
            foreach (AddEventHandler request in addRequests)
            {
                this.AddEventHandler(request.Handler);
            }
        }

        internal void AddEventHandler(EventHandler handler)
        {
            // if it is the first handler of a particular type of event, create a new handler set
            if (!this.eventHandlers.ContainsKey(handler.EventType))
            {
                this.eventHandlers.Add(handler.EventType, new HashSet<EventHandler>());
            }

            // add the event handler
            if (this.eventHandlers[handler.EventType].Add(handler))
            {
                // update the type cache
                TypeCache.Instance.Update(handler.EventType);

                // raise EventHandlerAddedToElement<> event
                Type eventType = typeof(EventHandlerAddedToElement<,>);
                Type[] parameters = { handler.GetType(), this.Type };
                object[] arguments = { handler, this };
                this.Raise(Event.New(eventType, parameters, parameters, arguments));
            }
        }

        internal void RemoveEventHandler(EventHandler handler)
        {
            // remove the event handler
            if (this.eventHandlers[handler.EventType].Remove(handler))
            {
                // if it is the last handler of a particular type of event, destroy the related handler set 
                if (this.eventHandlers[handler.EventType].Count == 0)
                {
                    this.eventHandlers.Remove(handler.EventType);
                }

                // raise EventHandlerRemovedFromElement<> event
                Type eventType = typeof(EventHandlerRemovedFrom<,>);
                Type[] parameters = { handler.GetType(), this.Type };
                object[] arguments = { handler, this };
                this.Raise(Event.New(eventType, parameters, parameters, arguments));
            }
        }

        #region IEventHandlersCollection Members

        void IEventHandlerCollection.Add(EventHandler handler)
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.Schedule(new AddEventHandler(handler));
        }

        void IEventHandlerCollection.Add<T>(EventHandler<T> handler)
        {
            this.EventHandlers.Add(new GenericEventHandler<T>(handler));
        }

        void IEventHandlerCollection.Remove(EventHandler handler)
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.Schedule(new RemoveEventHandler(handler));
        }

        void IEventHandlerCollection.Remove<T>(EventHandler<T> handler)
        {
            this.EventHandlers.Remove(new GenericEventHandler<T>(handler));
        }

        bool IEventHandlerCollection.Contains(EventHandler handler)
        {
            return (handler != null && this.eventHandlers.ContainsKey(handler.EventType) && this.eventHandlers[handler.EventType].Contains(handler));
        }

        bool IEventHandlerCollection.Contains<T>(EventHandler<T> handler)
        {
            return this.EventHandlers.Contains(new GenericEventHandler<T>(handler));
        }

        IEnumerable<EventHandler> IEventHandlerCollection.Get(Type eventType)
        {
            if (this.eventHandlers.ContainsKey(eventType))
            {
                return this.eventHandlers[eventType];
            }
            return Enumerable<EventHandler>.Empty;
        }

        IEnumerable<Type> IEventHandlerCollection.HandledTypes
        {
            get { return this.eventHandlers.Keys; }
        }

        #endregion
    }
}
