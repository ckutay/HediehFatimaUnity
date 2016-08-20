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

using SCG = System.Collections.Generic;

namespace ION.SyncCollections
{
    public interface IDictionary<K,V> : ICollectionValue<SCG.KeyValuePair<K,V>>
    {
        bool Contains(K key);

        bool ContainsAll<H>(SCG.IEnumerable<H> keys) where H : K;

        void Add(K key, V val);

        void AddAll<U, W>(SCG.IEnumerable<SCG.KeyValuePair<U, W>> entries)
            where U : K
            where W : V;

        void Remove(K key);

        void RemoveAll(SCG.IEnumerable<K> keys);

        void RemoveAll();

        ICollectionValue<K> Keys { get;}

        ICollectionValue<V> Values { get;}

        V this[K key] { get; set;}

    }
}
