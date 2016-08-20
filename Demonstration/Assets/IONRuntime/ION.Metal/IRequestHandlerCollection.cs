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

namespace ION.Meta
{
    public interface IRequestHandlerCollection
    {
        void Set(RequestHandler handler);

        void Set<T>(RequestHandler<T> handler)
            where T : Request;

        void Set<T1, T2>(RequestHandler<T1, T2> handler)
            where T1 : Request
            where T2 : Request;

        void Set<T1, T2, T3>(RequestHandler<T1, T2, T3> handler)
            where T1 : Request
            where T2 : Request
            where T3 : Request;

        void Set<T1, T2, T3, T4>(RequestHandler<T1, T2, T3, T4> handler)
            where T1 : Request
            where T2 : Request
            where T3 : Request
            where T4 : Request;

        void Set<T1, T2, T3, T4, T5>(RequestHandler<T1, T2, T3, T4, T5> handler)
            where T1 : Request
            where T2 : Request
            where T3 : Request
            where T4 : Request
            where T5 : Request;

        void Set<T1, T2, T3, T4, T5, T6>(RequestHandler<T1, T2, T3, T4, T5, T6> handler)
            where T1 : Request
            where T2 : Request
            where T3 : Request
            where T4 : Request
            where T5 : Request
            where T6 : Request;

        void Set<T1, T2, T3, T4, T5, T6, T7>(RequestHandler<T1, T2, T3, T4, T5, T6, T7> handler)
            where T1 : Request
            where T2 : Request
            where T3 : Request
            where T4 : Request
            where T5 : Request
            where T6 : Request
            where T7 : Request;

        void Set(RequestHandler<Request> handler, params Type[] requestTypes);

        void Clear(params Type[] requestTypes);

        void Clear<T>()
            where T : Request;

        void Clear<T1, T2>()
            where T1 : Request
            where T2 : Request;

        void Clear<T1, T2, T3>()
            where T1 : Request
            where T2 : Request
            where T3 : Request;

        void Clear<T1, T2, T3, T4>()
            where T1 : Request
            where T2 : Request
            where T3 : Request
            where T4 : Request;

        void Clear<T1, T2, T3, T4, T5>()
            where T1 : Request
            where T2 : Request
            where T3 : Request
            where T4 : Request
            where T5 : Request;

        void Clear<T1, T2, T3, T4, T5, T6>()
            where T1 : Request
            where T2 : Request
            where T3 : Request
            where T4 : Request
            where T5 : Request
            where T6 : Request;

        void Clear<T1, T2, T3, T4, T5, T6, T7>()
            where T1 : Request
            where T2 : Request
            where T3 : Request
            where T4 : Request
            where T5 : Request
            where T6 : Request
            where T7 : Request;

        bool Contains(RequestHandler handler);

        bool Contains<T>(RequestHandler<T> handler)
            where T : Request;

        bool Contains<T1, T2>(RequestHandler<T1, T2> handler)
            where T1 : Request
            where T2 : Request;

        bool Contains<T1, T2, T3>(RequestHandler<T1, T2, T3> handler)
            where T1 : Request
            where T2 : Request
            where T3 : Request;

        bool Contains<T1, T2, T3, T4>(RequestHandler<T1, T2, T3, T4> handler)
            where T1 : Request
            where T2 : Request
            where T3 : Request
            where T4 : Request;

        bool Contains<T1, T2, T3, T4, T5>(RequestHandler<T1, T2, T3, T4, T5> handler)
            where T1 : Request
            where T2 : Request
            where T3 : Request
            where T4 : Request
            where T5 : Request;

        bool Contains<T1, T2, T3, T4, T5, T6>(RequestHandler<T1, T2, T3, T4, T5, T6> handler)
            where T1 : Request
            where T2 : Request
            where T3 : Request
            where T4 : Request
            where T5 : Request
            where T6 : Request;

        bool Contains<T1, T2, T3, T4, T5, T6, T7>(RequestHandler<T1, T2, T3, T4, T5, T6, T7> handler)
            where T1 : Request
            where T2 : Request
            where T3 : Request
            where T4 : Request
            where T5 : Request
            where T6 : Request
            where T7 : Request;

        bool Contains(RequestHandler<Request> handler, params Type[] requestTypes);

        RequestHandler Get(params Type[] requestTypes);

        IEnumerable<Type[]> HandledTypes { get; }
    }
}
