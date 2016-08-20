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
using System.Linq;
using System.Collections.Generic;

namespace ION.Meta
{
    public static class IEnumerableExtensions
    {
        public static bool Are<T>(this Type[] source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                if (!source[i].IsSubclassOf(typeof(T)))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool HaveRepetitions(this Type[] source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                for (int j = i + 1; j < source.Length; j++)
                {
                    if (source[i] == source[j]) return true;
                }
            }
            return false;
        }

        public static bool UnorderedEqual(this Type[] source, Type[] other)
        {
            return (source.OrderBy(type => type.Name).SequenceEqual(other.OrderBy(type => type.Name)));
        }
    }

    public static class Enumerable<T>
    {
        public readonly static IEnumerable<T> Empty = new T[0];
    }
}
