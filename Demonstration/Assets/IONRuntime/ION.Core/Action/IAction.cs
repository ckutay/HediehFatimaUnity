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
//  29/05/2009      Guilherme Raimundo <guilherme.raimundo@inesc-id.pt>
//  First version.
//  ---  


namespace ION.Core
{
    /// <summary>
    /// When in the Running state, an Action raises a Step event for each Update step of the Simulation.
    /// Both the Idle and Paused states indicate that an Action is not running but the Pause state usually
    /// has some associated progression information.
    /// </summary>
    public enum ActionState : byte { Running, Idle, Paused }

    public interface IAction
    {
        /// <summary>
        /// Starts the Action.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the Action.
        /// </summary>
        /// <param name="success">
        /// Indicates whether the Action stops successfully or not.
        /// </param>
        void Stop(bool success);

        /// <summary>
        /// Pauses the Action.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the Action.
        /// </summary>
        void Resume();

        /// <summary>
        /// Gets the Action current State.
        /// </summary>
        ActionState CurrentState { get; }
    }
}