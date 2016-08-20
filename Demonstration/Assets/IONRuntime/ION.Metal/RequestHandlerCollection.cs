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
using System.Linq;
using ION.Meta.Events;
using ION.Meta.Requests;

namespace ION.Meta
{
    public partial class Element : IRequestHandlerCollection
    {
        private Dictionary<Type, RequestHandler> requestHandlers;

        private void SetupRequestHandlerCollection()
        {
            // setup the request handler collection
            this.requestHandlers = new Dictionary<Type, RequestHandler>();

            // setup the request handler for setting/clearing other request handlers
            this.SetRequestHandler(new GenericRequestHandler<SetRequestHandler, ClearRequestHandler>(this.UpdateRequestHandlerCollection, 1));
        }

        private void UpdateRequestHandlerCollection(IEnumerable<SetRequestHandler> setRequests, IEnumerable<ClearRequestHandler> clearRequests)
        {
            // process requests for clearing request handlers
            foreach (ClearRequestHandler request in clearRequests)
            {
                this.ClearRequestHandler(request.Handler);
            }

            // process requests for setting request handlers
            foreach (SetRequestHandler request in setRequests)
            {
                this.SetRequestHandler(request.Handler);
            }
        }

        internal void SetRequestHandler(RequestHandler handler)
        {
            if (!handler.RequestTypes.Any(this.requestHandlers.Keys.Contains))
            {
                // set the request handler (for each request type) 
                foreach (Type requestType in handler.RequestTypes)
                {
                    this.requestHandlers.Add(requestType, handler);
                }

                // raise RequestHandlerSetToElement<> event
                Type eventType = typeof(RequestHandlerSetToElement<,>);
                Type[] typeParameters = { handler.GetType(), this.Type };
                object[] arguments = { handler, this };
                this.Raise(Event.New(eventType, typeParameters, typeParameters, arguments));
            }
        }

        internal void ClearRequestHandler(RequestHandler handler)
        {
            if (this.requestHandlers.ContainsValue(handler))
            {
                // clear the request handler (for each request type) 
                foreach (Type requestType in handler.RequestTypes)
                {
                    this.requestHandlers.Remove(requestType);
                }

                // raise RequestHandlerClearedFromElement<> event
                Type eventType = typeof(RequestHandlerClearedFromElement<,>);
                Type[] typeParameters = { handler.GetType(), this.Type };
                object[] arguments = { handler, this };
                this.Raise(Event.New(eventType, typeParameters, typeParameters, arguments));
            }
        }

        #region IRequestHandlerCollection Members

        void IRequestHandlerCollection.Set(RequestHandler handler)
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.Schedule(new SetRequestHandler(handler));
        }

        void IRequestHandlerCollection.Set<T>(RequestHandler<T> handler)
        {
            this.RequestHandlers.Set(new GenericRequestHandler<T>(handler));
        }

        void IRequestHandlerCollection.Set<T1, T2>(RequestHandler<T1, T2> handler)
        {
            this.RequestHandlers.Set(new GenericRequestHandler<T1, T2>(handler));
        }

        void IRequestHandlerCollection.Set<T1, T2, T3>(RequestHandler<T1, T2, T3> handler)
        {
            this.RequestHandlers.Set(new GenericRequestHandler<T1, T2, T3>(handler));
        }

        void IRequestHandlerCollection.Set<T1, T2, T3, T4>(RequestHandler<T1, T2, T3, T4> handler)
        {
            this.RequestHandlers.Set(new GenericRequestHandler<T1, T2, T3, T4>(handler));
        }

        void IRequestHandlerCollection.Set<T1, T2, T3, T4, T5>(RequestHandler<T1, T2, T3, T4, T5> handler)
        {
            this.RequestHandlers.Set(new GenericRequestHandler<T1, T2, T3, T4, T5>(handler));
        }

        void IRequestHandlerCollection.Set<T1, T2, T3, T4, T5, T6>(RequestHandler<T1, T2, T3, T4, T5, T6> handler)
        {
            this.RequestHandlers.Set(new GenericRequestHandler<T1, T2, T3, T4, T5, T6>(handler));
        }

        void IRequestHandlerCollection.Set<T1, T2, T3, T4, T5, T6, T7>(RequestHandler<T1, T2, T3, T4, T5, T6, T7> handler)
        {
            this.RequestHandlers.Set(new GenericRequestHandler<T1, T2, T3, T4, T5, T6, T7>(handler));
        }

        void IRequestHandlerCollection.Set(RequestHandler<Request> handler, params Type[] requestTypes)
        {
            this.RequestHandlers.Set(new GenericRequestHandler(handler, requestTypes));
        }

        void IRequestHandlerCollection.Clear(params Type[] requestTypes)
        {
            RequestHandler handler = this.RequestHandlers.Get(requestTypes);
            if (handler != null)
            {
                this.Schedule(new ClearRequestHandler(handler));
            }
        }

        void IRequestHandlerCollection.Clear<T>()
        {
            this.RequestHandlers.Clear(typeof(T));
        }

        void IRequestHandlerCollection.Clear<T1, T2>()
        {
            this.RequestHandlers.Clear(typeof(T1), typeof(T2));
        }

        void IRequestHandlerCollection.Clear<T1, T2, T3>()
        {
            this.RequestHandlers.Clear(typeof(T1), typeof(T2), typeof(T3));
        }

        void IRequestHandlerCollection.Clear<T1, T2, T3, T4>()
        {
            this.RequestHandlers.Clear(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
        }

        void IRequestHandlerCollection.Clear<T1, T2, T3, T4, T5>()
        {
            this.RequestHandlers.Clear(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));
        }

        void IRequestHandlerCollection.Clear<T1, T2, T3, T4, T5, T6>()
        {
            this.RequestHandlers.Clear(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));
        }

        void IRequestHandlerCollection.Clear<T1, T2, T3, T4, T5, T6, T7>()
        {
            this.RequestHandlers.Clear(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));
        }

        bool IRequestHandlerCollection.Contains(RequestHandler handler)
        {
            return this.requestHandlers.ContainsValue(handler);
        }

        bool IRequestHandlerCollection.Contains<T>(RequestHandler<T> handler)
        {
            return this.requestHandlers.ContainsValue(new GenericRequestHandler<T>(handler));
        }

        bool IRequestHandlerCollection.Contains<T1, T2>(RequestHandler<T1, T2> handler)
        {
            return this.requestHandlers.ContainsValue(new GenericRequestHandler<T1, T2>(handler));
        }

        bool IRequestHandlerCollection.Contains<T1, T2, T3>(RequestHandler<T1, T2, T3> handler)
        {
            return this.requestHandlers.ContainsValue(new GenericRequestHandler<T1, T2, T3>(handler));
        }

        bool IRequestHandlerCollection.Contains<T1, T2, T3, T4>(RequestHandler<T1, T2, T3, T4> handler)
        {
            return this.requestHandlers.ContainsValue(new GenericRequestHandler<T1, T2, T3, T4>(handler));
        }

        bool IRequestHandlerCollection.Contains<T1, T2, T3, T4, T5>(RequestHandler<T1, T2, T3, T4, T5> handler)
        {
            return this.requestHandlers.ContainsValue(new GenericRequestHandler<T1, T2, T3, T4, T5>(handler));
        }

        bool IRequestHandlerCollection.Contains<T1, T2, T3, T4, T5, T6>(RequestHandler<T1, T2, T3, T4, T5, T6> handler)
        {
            return this.requestHandlers.ContainsValue(new GenericRequestHandler<T1, T2, T3, T4, T5, T6>(handler));
        }

        bool IRequestHandlerCollection.Contains<T1, T2, T3, T4, T5, T6, T7>(RequestHandler<T1, T2, T3, T4, T5, T6, T7> handler)
        {
            return this.requestHandlers.ContainsValue(new GenericRequestHandler<T1, T2, T3, T4, T5, T6, T7>(handler));
        }

        bool IRequestHandlerCollection.Contains(RequestHandler<Request> handler, params Type[] requestTypes)
        {
            return this.requestHandlers.ContainsValue(new GenericRequestHandler(handler, requestTypes));
        }

        RequestHandler IRequestHandlerCollection.Get(params Type[] requestTypes)
        {
            RequestHandler handler;
            if (requestTypes.Length > 0 && this.requestHandlers.TryGetValue(requestTypes[0], out handler))
            {
                if (requestTypes.UnorderedEqual(handler.RequestTypes))
                {
                    return handler;
                }
            }
            return null;
        }

        IEnumerable<Type[]> IRequestHandlerCollection.HandledTypes
        {
            get 
            {
                HashSet<Type[]> handledTypes = new HashSet<Type[]>();
                foreach (RequestHandler handler in this.requestHandlers.Values)
                {
                    handledTypes.Add(handler.RequestTypes);
                }
                return handledTypes;
            }
        }

        #endregion
    }
}
