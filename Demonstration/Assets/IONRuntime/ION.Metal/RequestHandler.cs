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
    public abstract class RequestHandler
    {
        private Type[] requestTypes;
        private ushort priority;

        protected RequestHandler(params Type[] requestTypes)
        {
            // check if the handled request types are Requests
            if (requestTypes.Length == 0 || !requestTypes.Are<Request>())
            {
                throw new ArgumentException("Invalid request types.", "requestTypes");
            }

            // check if there aren't repeated handled request types
            if (requestTypes.HaveRepetitions())
            {
                throw new ArgumentException("Repeated request types.", "requestTypes");
            }

            this.requestTypes = requestTypes.ToArray();
            this.priority = 0;
        }

        protected RequestHandler(ushort priority, params Type[] requestTypes)
            : this(requestTypes)
        {
            this.priority = priority;
        }

        public Type[] RequestTypes
        {
            get { return this.requestTypes; }
        }

        internal bool Handles(Request request)
        {
            return this.requestTypes.Contains(request.Type);
        }

        internal ushort Priority
        {
            get { return this.priority; }
            set { this.priority = value; }
        }

        public abstract void Invoke(IEnumerable<Request> requests);
    }

    public delegate void RequestHandler<T>(IEnumerable<T> r)
        where T : Request;

    public delegate void RequestHandler<T1, T2>(IEnumerable<T1> r1, IEnumerable<T2> r2)
        where T1 : Request
        where T2 : Request;

    public delegate void RequestHandler<T1, T2, T3>(IEnumerable<T1> r1, IEnumerable<T2> r2, IEnumerable<T3> r3)
        where T1 : Request
        where T2 : Request
        where T3 : Request;

    public delegate void RequestHandler<T1, T2, T3, T4>(IEnumerable<T1> r1, IEnumerable<T2> r2, IEnumerable<T3> r3, IEnumerable<T4> r4)
        where T1 : Request
        where T2 : Request
        where T3 : Request
        where T4 : Request;

    public delegate void RequestHandler<T1, T2, T3, T4, T5>(IEnumerable<T1> r1, IEnumerable<T2> r2, IEnumerable<T3> r3, IEnumerable<T4> r4, IEnumerable<T5> r5)
        where T1 : Request
        where T2 : Request
        where T3 : Request
        where T4 : Request
        where T5 : Request;

    public delegate void RequestHandler<T1, T2, T3, T4, T5, T6>(IEnumerable<T1> r1, IEnumerable<T2> r2, IEnumerable<T3> r3, IEnumerable<T4> r4, IEnumerable<T5> r5, IEnumerable<T6> r6)
        where T1 : Request
        where T2 : Request
        where T3 : Request
        where T4 : Request
        where T5 : Request
        where T6 : Request;

    public delegate void RequestHandler<T1, T2, T3, T4, T5, T6, T7>(IEnumerable<T1> r1, IEnumerable<T2> r2, IEnumerable<T3> r3, IEnumerable<T4> r4, IEnumerable<T5> r5, IEnumerable<T6> r6, IEnumerable<T7> r7)
        where T1 : Request
        where T2 : Request
        where T3 : Request
        where T4 : Request
        where T5 : Request
        where T6 : Request
        where T7 : Request;
}
