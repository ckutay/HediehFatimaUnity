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
namespace ION.Meta
{
    public struct SimulationTime
    {
        public enum PhaseID : byte { BeforeUpdate, ProcessingRequests, ProcessingEvents }

        private Simulation simulation;
        private ulong step;
        private PhaseID phase;

        public Simulation Simulation
        {
            get { return this.simulation; }
            internal set { this.simulation = value; }
        }

        public ulong Step
        {
            get { return this.step; }
            internal set { this.step = value; }
        }

        public PhaseID Phase
        {
            get { return this.phase; }
            internal set { this.phase = value; }
        }

        public bool IsEmpty
        {
            get { return (this.simulation == null); }
        }
    }
}
