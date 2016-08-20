//	ION Framework - Core Classes
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

namespace ION.Core.Events
{
	/// <remarks>
    /// Indicates that an Action has changed its state to Idle.
    /// </remarks>
    public interface IStopped : IStateChanged { }

	
    /// <remarks>
    /// Indicates that an Action has changed its state to Idle.
    /// </remarks>
    public interface IStopped<TAction> : IStateChanged<TAction>, IStopped
        where TAction : IAction { }
	
	/// <remarks>
    /// Indicates that an Action has stopped and reached its purpose.
    /// </remarks>
    public interface ISucceeded : IStopped { }
	
    /// <remarks>
    /// Indicates that an Action has stopped and reached its purpose.
    /// </remarks>
    public interface ISucceeded<TAction> : IStopped<TAction>, ISucceeded
        where TAction : IAction { }
	
	 /// <remarks>
    /// Indicates that an Action has stopped wihtout reaching its purpose.
    /// </remarks>
    public interface IFailed : IStopped { }
	
    /// <remarks>
    /// Indicates that an Action has stopped wihtout reaching its purpose.
    /// </remarks>
    public interface IFailed<TAction> : IStopped<TAction>, IFailed
        where TAction : IAction { }
}
