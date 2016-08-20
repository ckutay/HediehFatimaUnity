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
    public interface IEventHandlerCollection
    {
        void Add(EventHandler handler);

        void Add<T>(EventHandler<T> handler)
            where T : IEvent;

        void Remove(EventHandler handler);

        void Remove<T>(EventHandler<T> handler)
            where T : IEvent;

        bool Contains(EventHandler handler);

        bool Contains<T>(EventHandler<T> handler)
            where T : IEvent;

        IEnumerable<EventHandler> Get(Type eventType);

        IEnumerable<Type> HandledTypes { get; }
    }
}
