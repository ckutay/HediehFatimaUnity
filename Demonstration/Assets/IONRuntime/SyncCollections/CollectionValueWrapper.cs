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
	internal sealed class CollectionValueWrapper<T> : ICollectionValue<T>
	{
		public CollectionValueWrapper(C5.ICollectionValue<T> collectionValue)
		{
		    this.collectionValue = collectionValue;
		}

	    #region ICollectionValue<T> Members

        public int  Count
        {
	        get { return this.collectionValue.Count; }
        }

        public void  CopyTo(T[] arr, int startIndex)
        {
	        this.collectionValue.CopyTo(arr, startIndex);
        }

        public T[]  ToArray()
        {
            return this.collectionValue.ToArray();
        }
       
        #endregion

	    private readonly C5.ICollectionValue<T> collectionValue;

        #region IEnumerable<T> Members

        public SCG.IEnumerator<T> GetEnumerator()
        {
            return this.collectionValue.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable) this.collectionValue).GetEnumerator();
        }

        #endregion
    }
}
