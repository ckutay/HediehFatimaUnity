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
//  TODO: refactor
//
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ION.Meta
{
    public interface IEvent
    {
        SimulationTime RaiseTime { get; }
    }

    public abstract class Event : IEvent
    {
        private readonly Type type;
        private HashSet<Element> trail;
        private SimulationTime raiseTime;

        protected Event()
        {
            this.type = this.GetType();
            this.trail = new HashSet<Element>();
        }

        internal IEnumerable<Type> Types
        {
            get { return TypeCache.Instance[this.type]; }
        }

        internal HashSet<Element> Trail
        {
            get { return this.trail; }
        }

        public SimulationTime RaiseTime
        {
            get { return this.raiseTime; }
            internal set
            {
                if (this.raiseTime.IsEmpty)
                {
                    this.raiseTime = value;
                }
            }
        }

        private struct Key
        {
            public Key(Type genericTypeDef, Type[] typeArguments, Type[] typeParams)
            {
                this.GenericTypeDefinition = genericTypeDef;
                this.TypeArguments = typeArguments;
                this.TypeParameters = typeParams;
            }

            public readonly Type GenericTypeDefinition;
            public readonly Type[] TypeArguments;
            public readonly Type[] TypeParameters;
        }

        private class KeyComparer : IEqualityComparer<Key>
        {
            #region IEqualityComparer<Key> Members

            public bool Equals(Key x, Key y)
            {
                if (x.GenericTypeDefinition == y.GenericTypeDefinition)
                {
                    if (x.TypeParameters.Length == y.TypeParameters.Length)
                    {
                        for (int i = 0; i < x.TypeParameters.Length; i++)
                        {
                            if (x.TypeParameters[i] != y.TypeParameters[i])
                            {
                                return false;
                            }
                        }
                    }
                    if (x.TypeArguments.Length == y.TypeArguments.Length)
                    {
                        for (int i = 0; i < x.TypeArguments.Length; i++)
                        {
                            if (x.TypeArguments[i] != y.TypeArguments[i])
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }

            public int GetHashCode(Key obj)
            {
                return obj.GetHashCode();
            }

            #endregion
        }

        private readonly static C5.HashDictionary<Key, ConstructorInfo> Constructors = new C5.HashDictionary<Key, ConstructorInfo>(new KeyComparer());

        public static Event New(Type typeDefinition, Type[] typeArguments, object[] arguments)
        {
            return Event.New(typeDefinition, typeArguments, typeArguments, arguments);
        }

        /// <summary>
        /// Cached Constructor Info
        /// </summary>
        public static Event New(Type typeDefinition, Type[] typeArguments, Type[] parameters, object[] arguments)
        {
            ConstructorInfo constructor;
            var key = new Key(typeDefinition, typeArguments, parameters);

            //if (!Constructors.Find(key, out constructor))
            //{

                try
                {
                    // get the generic type
                    Type genericType = typeDefinition.MakeGenericType(typeArguments);

                    // get a constructor which matches the parameters
                    constructor = genericType.GetConstructor(
                         BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.ExactBinding,
                         null, CallingConventions.Any, parameters, null);

                   // Constructors.Add(key, constructor);
                }
                catch (Exception innerException)
                {
                    throw new Exception("Unable to create event.", innerException);
                }


            //}
            // invoke the constructor with the specified arguments and return the new event 
            return (Event)constructor.Invoke(arguments);
        }
    }    
}
