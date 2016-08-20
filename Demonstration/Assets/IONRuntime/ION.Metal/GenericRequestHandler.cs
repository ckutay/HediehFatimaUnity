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

namespace ION.Meta
{
    // wrapper for RequestHandler<T> delegates
    internal sealed class GenericRequestHandler<T> : RequestHandler
        where T : Request
    {
        private readonly RequestHandler<T> handler;

        public GenericRequestHandler(RequestHandler<T> handler)
            : base(typeof(T))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public GenericRequestHandler(RequestHandler<T> handler, ushort priority)
            : base(priority, typeof(T))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public override void Invoke(IEnumerable<Request> requests)
        {
            this.handler.Invoke(requests.OfType<T>());
        }

        public override bool Equals(object obj)
        {
            GenericRequestHandler<T> other = obj as GenericRequestHandler<T>;
            return (other != null && this.handler == other.handler);
        }

        public override int GetHashCode()
        {
            return handler.GetHashCode();
        }
    }

    // wrapper for RequestHandler<T1, T2> delegates
    internal sealed class GenericRequestHandler<T1, T2> : RequestHandler
        where T1 : Request
        where T2 : Request
    {
        private readonly RequestHandler<T1, T2> handler;

        public GenericRequestHandler(RequestHandler<T1, T2> handler)
            : base(typeof(T1), typeof(T2))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public GenericRequestHandler(RequestHandler<T1, T2> handler, ushort priority)
            : base(priority, typeof(T1), typeof(T2))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public override void Invoke(IEnumerable<Request> requests)
        {
            this.handler.Invoke(requests.OfType<T1>(), requests.OfType<T2>());
        }

        public override bool Equals(object obj)
        {
            GenericRequestHandler<T1, T2> other = obj as GenericRequestHandler<T1, T2>;
            return (other != null && this.handler == other.handler);
        }

        public override int GetHashCode()
        {
            return handler.GetHashCode();
        }
    }

    // wrapper for RequestHandler<T1, T2, T3> delegates
    internal sealed class GenericRequestHandler<T1, T2, T3> : RequestHandler
        where T1 : Request
        where T2 : Request
        where T3 : Request
    {
        private readonly RequestHandler<T1, T2, T3> handler;

        public GenericRequestHandler(RequestHandler<T1, T2, T3> handler)
            : base(typeof(T1), typeof(T2), typeof(T3))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public GenericRequestHandler(RequestHandler<T1, T2, T3> handler, ushort priority)
            : base(priority, typeof(T1), typeof(T2), typeof(T3))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public override void Invoke(IEnumerable<Request> requests)
        {
            this.handler.Invoke(requests.OfType<T1>(), requests.OfType<T2>(), requests.OfType<T3>());
        }

        public override bool Equals(object obj)
        {
            GenericRequestHandler<T1, T2, T3> other = obj as GenericRequestHandler<T1, T2, T3>;
            return (other != null && this.handler == other.handler);
        }

        public override int GetHashCode()
        {
            return handler.GetHashCode();
        }
    }

    // wrapper for RequestHandler<T1, T2, T3, T4> delegates
    internal sealed class GenericRequestHandler<T1, T2, T3, T4> : RequestHandler
        where T1 : Request
        where T2 : Request
        where T3 : Request
        where T4 : Request
    {
        private readonly RequestHandler<T1, T2, T3, T4> handler;

        public GenericRequestHandler(RequestHandler<T1, T2, T3, T4> handler)
            : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public GenericRequestHandler(RequestHandler<T1, T2, T3, T4> handler, ushort priority)
            : base(priority, typeof(T1), typeof(T2), typeof(T3), typeof(T4))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public override void Invoke(IEnumerable<Request> requests)
        {
            this.handler.Invoke(requests.OfType<T1>(), requests.OfType<T2>(), requests.OfType<T3>(), requests.OfType<T4>());
        }

        public override bool Equals(object obj)
        {
            GenericRequestHandler<T1, T2, T3, T4> other = obj as GenericRequestHandler<T1, T2, T3, T4>;
            return (other != null && this.handler == other.handler);
        }

        public override int GetHashCode()
        {
            return handler.GetHashCode();
        }
    }

    // wrapper for RequestHandler<T1, T2, T3, T4, T5> delegates
    internal sealed class GenericRequestHandler<T1, T2, T3, T4, T5> : RequestHandler
        where T1 : Request
        where T2 : Request
        where T3 : Request
        where T4 : Request
        where T5 : Request
    {
        private readonly RequestHandler<T1, T2, T3, T4, T5> handler;

        public GenericRequestHandler(RequestHandler<T1, T2, T3, T4, T5> handler)
            : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public GenericRequestHandler(RequestHandler<T1, T2, T3, T4, T5> handler, ushort priority)
            : base(priority, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public override void Invoke(IEnumerable<Request> requests)
        {
            this.handler.Invoke(requests.OfType<T1>(), requests.OfType<T2>(), requests.OfType<T3>(), requests.OfType<T4>(), requests.OfType<T5>());
        }

        public override bool Equals(object obj)
        {
            GenericRequestHandler<T1, T2, T3, T4, T5> other = obj as GenericRequestHandler<T1, T2, T3, T4, T5>;
            return (other != null && this.handler == other.handler);
        }

        public override int GetHashCode()
        {
            return handler.GetHashCode();
        }
    }

    // wrapper for RequestHandler<T1, T2, T3, T4, T5, T6> delegates
    internal sealed class GenericRequestHandler<T1, T2, T3, T4, T5, T6> : RequestHandler
        where T1 : Request
        where T2 : Request
        where T3 : Request
        where T4 : Request
        where T5 : Request
        where T6 : Request
    {
        private readonly RequestHandler<T1, T2, T3, T4, T5, T6> handler;

        public GenericRequestHandler(RequestHandler<T1, T2, T3, T4, T5, T6> handler)
            : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public GenericRequestHandler(RequestHandler<T1, T2, T3, T4, T5, T6> handler, ushort priority)
            : base(priority, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public override void Invoke(IEnumerable<Request> requests)
        {
            this.handler.Invoke(requests.OfType<T1>(), requests.OfType<T2>(), requests.OfType<T3>(), requests.OfType<T4>(), requests.OfType<T5>(), requests.OfType<T6>());
        }

        public override bool Equals(object obj)
        {
            GenericRequestHandler<T1, T2, T3, T4, T5, T6> other = obj as GenericRequestHandler<T1, T2, T3, T4, T5, T6>;
            return (other != null && this.handler == other.handler);
        }

        public override int GetHashCode()
        {
            return handler.GetHashCode();
        }
    }

    // wrapper for RequestHandler<T1, T2, T3, T4, T5, T6, T7> delegates
    internal sealed class GenericRequestHandler<T1, T2, T3, T4, T5, T6, T7> : RequestHandler
        where T1 : Request
        where T2 : Request
        where T3 : Request
        where T4 : Request
        where T5 : Request
        where T6 : Request
        where T7 : Request
    {
        private readonly RequestHandler<T1, T2, T3, T4, T5, T6, T7> handler;

        public GenericRequestHandler(RequestHandler<T1, T2, T3, T4, T5, T6, T7> handler)
            : base(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public GenericRequestHandler(RequestHandler<T1, T2, T3, T4, T5, T6, T7> handler, ushort priority)
            : base(priority, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public override void Invoke(IEnumerable<Request> requests)
        {
            this.handler.Invoke(requests.OfType<T1>(), requests.OfType<T2>(), requests.OfType<T3>(), requests.OfType<T4>(), requests.OfType<T5>(), requests.OfType<T6>(), requests.OfType<T7>());
        }

        public override bool Equals(object obj)
        {
            GenericRequestHandler<T1, T2, T3, T4, T5, T6, T7> other = obj as GenericRequestHandler<T1, T2, T3, T4, T5, T6, T7>;
            return (other != null && this.handler == other.handler);
        }

        public override int GetHashCode()
        {
            return handler.GetHashCode();
        }
    }

    // wrapper for RequestHandler<Request> delegates
    internal sealed class GenericRequestHandler : RequestHandler
    { 
        private readonly RequestHandler<Request> handler;

        public GenericRequestHandler(RequestHandler<Request> handler, params Type[] requestTypes)
            : base(requestTypes)
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public GenericRequestHandler(RequestHandler<Request> handler, ushort priority, params Type[] requestTypes)
            : base(priority, requestTypes)
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public override void Invoke(IEnumerable<Request> requests)
        {
            this.handler.Invoke(requests);
        }

        public override bool Equals(object obj)
        {
            GenericRequestHandler other = obj as GenericRequestHandler;
            return (other != null && this.handler == other.handler);
        }

        public override int GetHashCode()
        {
            return handler.GetHashCode();
        }
    }
}
