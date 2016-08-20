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

namespace ION.Meta
{
    public sealed class TypeCache
    {
        // singleton
        public static readonly TypeCache Instance = new TypeCache();

        private readonly C5.HashDictionary<Type, C5.LinkedList<Type>> assignableTypesFrom;

        private TypeCache()
        {
            this.assignableTypesFrom = new C5.HashDictionary<Type, C5.LinkedList<Type>>();
        }

        public Type[] Update(Type baseType)
        {
            C5.LinkedList<Type> types;
            if (!this.assignableTypesFrom.Find(baseType, out types))
            {
                types = new C5.LinkedList<Type>();
                this.assignableTypesFrom[baseType] = types;

                foreach (Type type in this.assignableTypesFrom.Keys)
                {
                    if (type.IsAssignableFrom(baseType))
                    {
                        this.assignableTypesFrom[baseType].Add(type);
                    }
                    else if (baseType.IsAssignableFrom(type))
                    {
                        this.assignableTypesFrom[type].Add(baseType);
                    }
                }
            }
            return types.ToArray();
        }

        public Type[] this[Type baseType]
        {
            get { return Update(baseType); }
        }
    }
}
