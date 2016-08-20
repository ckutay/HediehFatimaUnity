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
//  TODO: recheck update cycle, look at destroy
//
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ION.Meta.Requests;
using ION.Meta.Events;


namespace ION.Meta
{
    public abstract partial class Element
    {
        private readonly Type type;
        private readonly ulong uid;
        private Simulation simulation;
        private bool wasDestroyed;

        private LinkedList<Request> requests;
        private Request[] lockedRequests;

        private LinkedList<Event> events;

        internal Element(ulong uid)
        {
            this.type = this.GetType();
            this.uid = uid;
            this.simulation = null;
            this.wasDestroyed = false;

            // setup requests queue
            this.requests = new LinkedList<Request>();

            // setup events queue
            this.events = new LinkedList<Event>();

            // setup handlers' collections
            this.SetupRequestHandlerCollection();
            this.SetupEventHandlerCollection();
        }

        protected Element()
            : this(Simulator.GenerateUID())
        {
        }

        public void Destroy()
        {
            if (this.simulation == null)
            {
                this.wasDestroyed = true;
                this.requests = null;
                this.events = null;

                this.OnDestroy();
            }
        }

        public abstract void OnDestroy();

        internal Type Type
        {
            get { return this.type; }
        }

        public ulong UID
        {
            get { return this.uid; }
        }

        public bool WasDestroyed
        {
            get { return this.wasDestroyed; }
        }

        public Simulation Simulation
        {
            get { return this.simulation; }
            internal set { this.simulation = value; }
        }


        #region Requests

        public IRequestHandlerCollection RequestHandlers
        {
            get { return this; }
        }

        public void Schedule(Request request)
        {
            if (this.wasDestroyed)
            {
                return;
            }

            if (this.simulation != null)
            {
                // add time stamp
                request.ScheduleTime = this.simulation.Time;

                // enqueue request
                this.requests.AddLast(request);

                this.simulation.SynchronizeProcessRequests(this);
            }
            else
            {
                // process immediatly (not synchronized)
                this.Process(request);
            }
        }

        HashSet<RequestHandler> hands;


        internal void Process(Request request)
        {
            RequestHandler handler;
            if (this.requestHandlers.TryGetValue(request.Type, out handler))
            {
                handler.Invoke(new[] { request });
            }
        }

        internal void LockRequests()
        {
			lock(this.requests)
			{
	            // lock the number of pending requests before processing
	            // (new requests scheduled during processing will be processed in the next update)
	            this.lockedRequests = this.requests.ToArray();
	            this.requests.Clear();
	
	            hands = new HashSet<RequestHandler>();
	            foreach (Request r in this.lockedRequests)
	            {
	                hands.Add(this.requestHandlers[r.Type]);
	            }
			}
        }


        internal void ProcessRequests(ushort priority = 0)
        {
            IEnumerable<RequestHandler> handlers = this.hands.Where(handler => handler.Priority == priority);

            foreach (RequestHandler handler in handlers)
            {
                // filter requests that match the handler's request types
                IEnumerable<Request> reqs = this.lockedRequests.Where(handler.Handles);

                if (reqs.Count() > 0)
                {
                    handler.Invoke(reqs);
                }
            }
        }

        #endregion



        #region Events


        public IEventHandlerCollection EventHandlers
        {
            get { return this; }
        }

        public void Raise(Event e)
        {
            if (this.wasDestroyed)
            {
                return;
            }

            if (e.Trail.Add(this))
            {
                if (this.simulation != null)
                {
                    // add time stamp
                    e.RaiseTime = this.simulation.Time;

                    // enqueue event
                    this.events.AddLast(e);

                    // synchronize with the simulation
                    this.simulation.SynchronizeProcessEvents(this);

                    // propagate to the simulation
                    this.simulation.Raise(e);
                }
                else
                {
                    // process event immediatly (not synchronized)
                    this.Process(e);
                }
            }
        }

        internal void ProcessEvents()
        {
            // arranjar um mecanismo que quebre o loop (por batch com switch entre elementos)
            while (events.Count > 0)
            {
                this.Process(events.First.Value);
                events.RemoveFirst();
            }
        }

        internal void Process(Event evt)
        {
            if (this.eventHandlers != null && this.eventHandlers.Count > 0)
            {
                foreach (Type type in evt.Types)
                {
                    if (this.eventHandlers.ContainsKey(type))
                    {
                        foreach (EventHandler handler in this.eventHandlers[type])
                        {
                            handler.Invoke(evt);
                        }
                    }
                }
            }
        }

        #endregion

    }
}
