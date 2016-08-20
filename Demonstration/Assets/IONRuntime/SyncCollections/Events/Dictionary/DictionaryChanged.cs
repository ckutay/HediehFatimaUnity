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

using ION.Meta;

namespace ION.SyncCollections.Events.Dictionary
{
    internal class DictionaryChanged<TDictionary> : Event, IDictionaryChanged<TDictionary>
    {
        public DictionaryChanged(TDictionary dictionary)
        {
            this.dictionary = dictionary;
        }

        public DictionaryChanged(Event evt)
        {
            IDictionaryChanged dictionaryChanged;
            if((dictionaryChanged = evt as IDictionaryChanged) != null)
            {
                this.dictionary = (TDictionary) dictionaryChanged.Dictionary;
            }
        }

        object IDictionaryChanged.Dictionary
        {
            get { return this.dictionary; }
        }
        public TDictionary Dictionary
        {
            get { return this.dictionary; }
        }
        internal readonly TDictionary dictionary;
    }

    internal class DictionaryChanged<TKey, TValue, TDictionary> : DictionaryChanged<TDictionary>, IDictionaryChangedInternal
    {
        public DictionaryChanged(TKey key, TValue value, TDictionary dictionary)
            : base(dictionary)
        {
            this.key = key;
            this.value = value;
        }
      
        public DictionaryChanged(Event evt)
            : base(evt)
        {
            IDictionaryChangedInternal dictionaryChanged;
            if((dictionaryChanged = evt as IDictionaryChangedInternal) != null)
            {
                this.key = (TKey) dictionaryChanged.Key;
                this.value = (TValue) dictionaryChanged.Value;
            }
        }

        public TKey Key
        {
            get { return this.key; }
        }
        internal readonly TKey key;

        
        public TValue Value
        {
            get { return this.value; }
        }
        internal readonly TValue value;

        #region IDictionaryChangedInternal Members

        object IDictionaryChangedInternal.Dictionary
        {
            get {return this.dictionary; }
        }

        object IDictionaryChangedInternal.Key
        {
            get { return this.key; }
        }

        object IDictionaryChangedInternal.Value
        {
            get { return this.value; }
        }

        #endregion
    }
}
