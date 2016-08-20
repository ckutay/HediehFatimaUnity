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
using System.Collections.Generic;
using System.Linq;
using ION.Meta.Events;

namespace ION.Meta
{
    public sealed partial class Simulation : Element
    {
        // singleton
        public static readonly Simulation Instance = new Simulation();

        private SimulationTime time;
        private readonly HashSet<Element> pendingProcessRequests;
        private readonly HashSet<Element> pendingProcessEvents;
        private readonly HashSet<Element> pendingRemovals;
        Element[] elementsToProcess;


        private Simulation() 
            : base(Simulator.GenerateUID())
        {
            // setup simulation time
            this.time.Simulation = this;
            this.time.Step = 0;
            this.time.Phase = SimulationTime.PhaseID.BeforeUpdate;

            // setup synchronization lists
            this.pendingProcessRequests = new HashSet<Element>();
            this.pendingProcessEvents = new HashSet<Element>();
            this.pendingRemovals = new HashSet<Element>();

            // setup element collection
            this.SetupElementCollection();
            this.AddElement(this);
        }

        public IElementCollection Elements
        {
            get { return this; }
        }




        public override void OnDestroy()
        {
        }

        public SimulationTime Time
        {
            get { return this.time; }
        }


        internal void SynchronizeProcessRequests(Element element)
        {
            this.pendingProcessRequests.Add(element);
        }

        internal void SynchronizeProcessEvents(Element element)
        {
            this.pendingProcessEvents.Add(element);            
        }

        internal void SynchronizeRemoval(Element element)
        {
            this.pendingProcessRequests.Remove(element);
            this.pendingProcessEvents.Remove(element);
            this.pendingRemovals.Add(element);
        }

        public void Update()
        {     
            // process requests
            this.time.Phase = SimulationTime.PhaseID.ProcessingRequests;
            this.ProcessRequests();

            // process removals
            this.ProcessRemovals();

            // raise event of simulation updated
            this.Raise(new SimulationUpdated(this, this.time.Step));

            // process events
            this.time.Phase = SimulationTime.PhaseID.ProcessingEvents;
            this.ProcessEvents();

            // update time
            this.time.Step++;
            this.time.Phase = SimulationTime.PhaseID.BeforeUpdate;
        }

        private void ProcessRequests()
        {
            // process requests
            if (this.pendingProcessRequests.Count > 0)
            {
                elementsToProcess = this.pendingProcessRequests.ToArray();
                this.pendingProcessRequests.Clear();

                foreach (Element element in this.elementsToProcess)
                {
                    element.LockRequests();
                }
                foreach (Element element in this.elementsToProcess)
                {
                    element.ProcessRequests(0);
                }
                foreach (Element element in this.elementsToProcess)
                {
                    element.ProcessRequests(1);
                }
            }
        }

        private new void ProcessEvents()
        {
            // process events
            while (this.pendingProcessEvents.Count > 0)
            {
                elementsToProcess = this.pendingProcessEvents.ToArray();
                this.pendingProcessEvents.Clear();
                foreach (Element element in this.elementsToProcess)
                {
                    element.ProcessEvents();
                }
            }
        }

        private void ProcessRemovals()
        {
            // process removals
            if (this.pendingRemovals.Count > 0)
            {
                foreach (Element element in this.pendingRemovals)
                {
                    element.LockRequests();
                    element.ProcessRequests(0);
                    element.ProcessRequests(1);
                    element.ProcessEvents();
                }
                this.pendingRemovals.Clear();
            }
        }
    }
}
