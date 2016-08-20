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

namespace ION.Meta
{
    // wrapper for EventHandler<T> delegates
    internal class GenericEventHandler<T> : EventHandler
        where T : IEvent
    {
        private readonly EventHandler<T> handler;

        public GenericEventHandler(EventHandler<T> handler)
            : base(typeof(T))
        {
            // check if the handler is not null
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }
            this.handler = handler;
        }

        public override void Invoke(IEvent e)
        {
            this.handler.Invoke((T)e);
        }

        public override bool Equals(object obj)
        {
            GenericEventHandler<T> other = obj as GenericEventHandler<T>;
            return (other != null && this.handler == other.handler);
        }

        public override int GetHashCode()
        {
            return handler.GetHashCode();
        }
    }
}
