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
//  30/04/2009      Guilherme Raimundo <guilherme.raimundo@inesc-id.pt>
//  Corrected CollectionModified Exception error on RemoveAll() method when the dictionary was outside a simulation. 
//  ---
//  29/05/2009      Guilherme Raimundo <guilherme.raimundo@inesc-id.pt>
//  Solved an error of Events of dictionary state changed being raised before the actual change took place. 
//  This error only occured if the dictionary was was outside the Simulation. I.E. when changes were made assynchronously.
//  

using System;
using System.Linq;
using ION.Meta;
using ION.SyncCollections.Events.Dictionary;
using SCG = System.Collections.Generic;

namespace ION.SyncCollections
{
    public class HashDictionary<TKey, TValue> : Element, IDictionary<TKey,TValue> 
    {
        public HashDictionary()
        {
            this.RequestHandlers.Set(this.HandleModificationRequests,typeof(AddRequest),typeof(RemoveRequest),typeof(SetRequest));
        }

        #region Requests
        protected abstract class ModifyElementRequest : Request
        {
            protected ModifyElementRequest(TKey key)
            {
                this.key = key;
            }

            public readonly TKey key;
        }

        protected sealed class AddRequest : ModifyElementRequest
        {
            public AddRequest(TKey key, TValue value)
                : base(key)
            {
                this.value = value;
            }
            public readonly TValue value;
        }

        protected sealed class SetRequest : ModifyElementRequest
        {
            public SetRequest(TKey key, TValue value)
                : base(key)
            {
                this.value = value;
            }
            public readonly TValue value;
        }

        protected sealed class RemoveRequest : ModifyElementRequest
        {
            public RemoveRequest(TKey key)
                : base(key) {}
        }

        public override void OnDestroy() {}

        #endregion

        #region Request Handlers
        /// <summary>
        /// Sets have maximum priority. I.E. All elements on setRequests are removed from the add and remove requests list.
        /// </summary>
        protected void HandleModificationRequests(SCG.IEnumerable<Request> requests)
        {
            bool hadElements = !this.Dictionary.IsEmpty;

            C5.HashSet<ModifyElementRequest> setRequestsVerified, addRequestsVerified, removeRequestsVerified;

            this.SelectRequests(requests, out setRequestsVerified, out addRequestsVerified, out removeRequestsVerified);

            this.ExecuteSet(setRequestsVerified);
            this.ExecuteAdd(addRequestsVerified);
            this.ExecuteRemove(removeRequestsVerified);
            

            if (hadElements && this.Dictionary.IsEmpty)
            {
                this.Raise(Event.New(typeof(Cleared<>), new[] {this.GetType()}, new object[] {this}));
            }
        }

        private sealed class ModifyRequestComparer : SCG.IEqualityComparer<ModifyElementRequest>
        {
            private ModifyRequestComparer() { }

            public static readonly ModifyRequestComparer Instance = new ModifyRequestComparer();

            #region IEqualityComparer<T> Members

            public bool Equals(ModifyElementRequest x, ModifyElementRequest y)
            {
                return x.key.Equals(y.key);
            }

            public int GetHashCode(ModifyElementRequest obj)
            {
                return obj.key.GetHashCode();
            }

            #endregion
        }

        /// <summary>
        /// Changes the received collections by cleaning requests according to the following policy:
        /// if an object is Set then it is not Added
        /// if an object is Set then it is not Removed
        /// if an object is Added then it is not Removed
        /// </summary>
        private void SelectRequests(SCG.IEnumerable<Request> allRequests,
            out C5.HashSet<ModifyElementRequest> setRequests, out C5.HashSet<ModifyElementRequest> addRequests,
            out C5.HashSet<ModifyElementRequest> removeRequests)
        {
            C5.HashSet<ModifyElementRequest> setRequestsVerified = SelectAndRemoveRepeated<SetRequest>(allRequests);
            C5.HashSet<ModifyElementRequest> addRequestsVerified = SelectAndRemoveRepeated<AddRequest>(allRequests);
            C5.HashSet<ModifyElementRequest> removeRequestsVerified = SelectAndRemoveRepeated<RemoveRequest>(allRequests);

            foreach (ModifyElementRequest setRequest in setRequestsVerified)
            {
                addRequestsVerified.Remove(setRequest); // if an object is to be Set then it is not Added
                removeRequestsVerified.Remove(setRequest); // if an object is to be Set then it is not Removed 
            }

            foreach(ModifyElementRequest addRequest in addRequestsVerified)
            {
                if(!this.Dictionary.Contains(addRequest.key))
                {
                    removeRequestsVerified.Remove(addRequest); // if an object is to be Added and is not already
                                                               // in the collection then it is not Removed
                }
            }

            setRequests = setRequestsVerified;
            addRequests = addRequestsVerified;
            removeRequests = removeRequestsVerified;
        }

        private static C5.HashSet<ModifyElementRequest> SelectAndRemoveRepeated<TRequest>(SCG.IEnumerable<Request> allRequests)
            where TRequest : ModifyElementRequest
        {
            C5.HashSet<ModifyElementRequest> requestsVerified;
            SCG.IEnumerable<TRequest> requestsTemp = allRequests.OfType<TRequest>();
            int tempCount = requestsTemp.Count();
			if (tempCount > 0)
            {
                requestsVerified = new C5.HashSet<ModifyElementRequest>(tempCount, ModifyRequestComparer.Instance);
                requestsVerified.AddAll(requestsTemp); // Removes requests with same key
            }
            else
            {
                requestsVerified = new C5.HashSet<ModifyElementRequest>(ModifyRequestComparer.Instance);
            }
            return requestsVerified;
        }

        /// <summary>
        /// Assumes there are no duplicate requests. i.e. requests with the same key.
        /// </summary>
        private void ExecuteSet(SCG.IEnumerable<ModifyElementRequest> requests)
        {
            foreach (SetRequest request in requests)
            {
                TValue oldValue;
                bool raiseRemoved = false;
                Event removedEvent = null;
                if(this.Dictionary.Find(request.key, out oldValue))
                {
                    if(oldValue.Equals(request.value)) //When setting the same key/value do nothing
                    {
                        continue;
                    }

                    raiseRemoved = true;
                    removedEvent = Event.New(typeof (Removed<,,>),
                                             new[] {request.key.GetType(), oldValue.GetType(), this.GetType()},
                                             new object[] {request.key, oldValue, this});
                    
                }

                this.Dictionary[request.key] = request.value;
                if(raiseRemoved)
                {
                    this.Raise(removedEvent);
                }
                this.Raise(Event.New(typeof(Added<,,>),
                                        new[] { request.key.GetType(), request.value.GetType(), this.GetType() },
                                        new object[] { request.key, request.value, this }));
            }
        }

        /// <summary>
        /// Assumes there are no duplicate requests. i.e. requests with the same key.
        /// </summary>
        private void ExecuteAdd(SCG.IEnumerable<ModifyElementRequest> requests)
        {
            foreach (AddRequest request in requests)
            {
                TValue reqValue = request.value;
                if (!this.Dictionary.FindOrAdd(request.key, ref reqValue))
                {
                    this.Raise(Event.New(typeof (Added<,,>),
                                         new[] {request.key.GetType(), request.value.GetType(), this.GetType()},
                                         new object[] {request.key, request.value, this}));
                }
            }
        }
        
        /// <summary>
        /// Assumes there are no duplicate requests. i.e. requests with the same key.
        /// </summary>
        private void ExecuteRemove(SCG.IEnumerable<ModifyElementRequest> requests)
        {
            foreach (RemoveRequest request in requests)
            {
                TValue value;
                if(this.Dictionary.Remove(request.key, out value))
                {
                    this.Raise(Event.New(typeof(Removed<,,>),
                                         new[] { request.key.GetType(), value.GetType(), this.GetType() },
                                         new object[] { request.key, value, this }));
                }
            }
        }
        
        #endregion
        

        #region IDictionary<K,V> Members

        #region Modifiers

        public void Add(TKey key, TValue val)
        {
            this.Schedule(new AddRequest(key, val));
        }

        public void AddAll<U, W>(SCG.IEnumerable<SCG.KeyValuePair<U, W>> entries)
            where U : TKey
            where W : TValue
        {
            foreach (SCG.KeyValuePair<U, W> pair in entries)
            {
                this.Schedule(new AddRequest(pair.Key,pair.Value));
            }
        }

        public void Remove(TKey key)
        {
            this.Schedule(new RemoveRequest(key));
        }

        public void RemoveAll(System.Collections.Generic.IEnumerable<TKey> keys)
        {
            foreach (TKey key in keys)
            {
                this.Schedule(new RemoveRequest(key));
            }
        }

        public void RemoveAll()
        {
            TKey[] keys = this.Dictionary.Keys.ToArray();
            foreach (TKey key in keys)
            {
                this.Schedule(new RemoveRequest(key));
            }
        }

        #endregion

        public bool Contains(TKey key)
        {
            return this.Dictionary.Contains(key);
        }

        public bool ContainsAll<H>(System.Collections.Generic.IEnumerable<H> keys) where H : TKey
        {
            return this.Dictionary.ContainsAll(keys);
        }
       
        public ICollectionValue<TKey> Keys
        {
            get { return new CollectionValueWrapper<TKey>(this.Dictionary.Keys); }
        }

        public ICollectionValue<TValue> Values
        {
            get { return new CollectionValueWrapper<TValue>(this.Dictionary.Values); }
        }

        public TValue this[TKey key]
        {
            get
            {
                return this.Dictionary[key];
            }
            set
            {
                this.Schedule(new SetRequest(key, value));
            }
        }

        #endregion

        #region ICollectionValue<KeyValuePair<K,V>> Members

        public bool Contains(SCG.KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            if (this.Dictionary.Find(item.Key, out value))
            {
                return value.Equals(item.Value);
            }
            return false;
        }

        public int Count
        {
            get { return this.Dictionary.Count; }
        }
        

        public void CopyTo(SCG.KeyValuePair<TKey, TValue>[] arr, int startIndex)
        {
            C5.KeyValuePair<TKey, TValue>[] nativeArr = this.Dictionary.ToArray();
            if ((arr.Length - startIndex) < nativeArr.Length)
            {
                throw new ArgumentOutOfRangeException("arr",arr,"Array does not have sufficient space starting from index " + startIndex);
            }

            for (int i = startIndex, j = 0; j < nativeArr.Length; i++, j++)
            {
                arr[i] = new SCG.KeyValuePair<TKey,TValue>(nativeArr[j].Key, nativeArr[j].Value);
            }
        }

        public SCG.KeyValuePair<TKey, TValue>[] ToArray()
        {
            C5.LinkedList<SCG.KeyValuePair<TKey,TValue>> convertList = new C5.LinkedList<SCG.KeyValuePair<TKey, TValue>>();

            foreach (C5.KeyValuePair<TKey, TValue> pair in this.Dictionary)
            {
                convertList.Add(new SCG.KeyValuePair<TKey,TValue>(pair.Key, pair.Value));
            }
            
            return convertList.ToArray();
        }

        #endregion

        #region IEnumerable<KeyValuePair<K,V>> Members

        public SCG.IEnumerator<SCG.KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            C5.LinkedList<SCG.KeyValuePair<TKey, TValue>> convertList = new C5.LinkedList<SCG.KeyValuePair<TKey, TValue>>();

            foreach (C5.KeyValuePair<TKey, TValue> pair in this.Dictionary)
            {
                convertList.Add(new SCG.KeyValuePair<TKey, TValue>(pair.Key, pair.Value));
            }

            return convertList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Dictionary.GetEnumerator();
        }

        #endregion

        private C5.HashDictionary<TKey,TValue> dictionary;
        
        protected C5.HashDictionary<TKey,TValue> Dictionary
        {
            get
            { 
                if(this.dictionary == null)
                {
                    this.dictionary = new C5.HashDictionary<TKey, TValue>(); //lazy creation
                }
                return this.dictionary;
            }
        }
    }
}
