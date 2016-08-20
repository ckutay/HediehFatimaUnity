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
    public interface ICollectionValue<TItem> : SCG.IEnumerable<TItem>
    {
        //bool Contains(T item);

        int Count { get;}

        //bool Find(ref T item);

        void CopyTo(TItem[] arr, int startIndex);

        TItem[] ToArray();

        //bool Find(System.Predicate<T> p, out T itemFound);

        //bool All(System.Predicate<T> p);

        //bool Exists(System.Predicate<T> p);

        //void Apply(System.Action<T> a);

        //SCG.IEnumerable<T> Filter(System.Predicate<T> p);

    }
}
