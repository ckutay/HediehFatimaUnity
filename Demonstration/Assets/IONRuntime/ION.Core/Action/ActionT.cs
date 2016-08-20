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
//  29/05/2009      Guilherme Raimundo <guilherme.raimundo@inesc-id.pt>
//  Solved an error of Events of state changed being raised before the actual change took place. 
//  This error only occured if the Action was outside the Simulation. I.E. when changes were made assynchronously.
//  

using System;
using System.Linq;
using ION.Core.Events;
using ION.Meta;
using System.Collections.Generic;

namespace ION.Core
{
    public class Action<TStartArguments> : Action, IAction<TStartArguments>
    {
        internal override Type[] HandledTypes
        {
            get
            {
                return new Type[] {typeof (StartRequest), typeof (StartArgumentsRequest),
                                   typeof (StopSuccessRequest), typeof (StopFailRequest),
                                   typeof (PauseRequest), typeof (ResumeRequest),
                                   typeof (StepRequest)};
            }
        }

        #region Requests
        protected sealed class StartArgumentsRequest : Request
        {
            public StartArgumentsRequest(TStartArguments arguments)
            {
                this.Arguments = arguments;
            }
            public readonly TStartArguments Arguments;
        }
        #endregion

        #region Request Handlers
        /// <summary>
        /// Calls the appropriate handler according to the state of the action.
        /// Has the following policy:
        ///     Idle State Request Priorities:
        ///         StartArguments > Start > Other requests
        ///     Running State Request Priorities:
        ///         Stop Fail > Stop Sucess > Pause > Step > Other Requests
        ///     Paused State Request Priorities:
        ///         Stop Fail > Stop Success > Resume > Other Requests
        /// </summary>
        protected override void HandleRequests(IEnumerable<Request> requests)
        {
            switch (this.state)
            {
                case ActionState.Idle:
                    this.ExecuteIdleState(requests.OfType<StartRequest>().ToArray(), requests.OfType<StartArgumentsRequest>().ToArray());
                    break;
                case ActionState.Running:
                    this.ExecuteRunningState(requests.OfType<StopSuccessRequest>().ToArray(), requests.OfType<StopFailRequest>().ToArray(), requests.OfType<PauseRequest>().ToArray(), requests.OfType<StepRequest>().ToArray());
                    break;
                case ActionState.Paused:
                    this.ExecutePausedState(requests.OfType<StopSuccessRequest>().ToArray(), requests.OfType<StopFailRequest>().ToArray(), requests.OfType<ResumeRequest>().ToArray());
                    break;
                default:
                    throw new Exception("Unknown State Type: " + this.state);
            }
        }

        private void ExecuteIdleState(StartRequest[] startRequests, StartArgumentsRequest[] startArgumentsRequests)
        {
            foreach (StartArgumentsRequest request in startArgumentsRequests)
            {
                this.ExecuteStartArguments(request.Arguments); //Choose the first start
                return;
            }

            if (startRequests.Length > 0)
            {
                this.startArguments = default(TStartArguments);
                this.ExecuteStart();
                return;
            }
        }

        private void ExecuteStartArguments(TStartArguments arguments)
        {
            this.EventChangedStateArguments[1] = this.state;
            this.startArguments = arguments;
            this.state = ActionState.Running;
            this.Raise(Event.New(typeof(Started<>), this.EventTypeParams, this.EventTypeArguments, this.EventChangedStateArguments));
        }

        #endregion

        /// <summary>
        /// Starts the Action with a specific starting arguments.
        /// </summary>
        /// <param name="arguments">Starting arguments.</param>
        public void Start(TStartArguments arguments)
        {
            this.Schedule(new StartArgumentsRequest(arguments));
        }

        /// <summary>
        /// Gets the Argument with which the action was started.
        /// </summary>
        public TStartArguments StartArguments
        {
            get { return startArguments; }
        }

        /// <summary>
        /// Direcly changes the value of the Action starting arguments without regarding the synchronization cycle. 
        /// Only change this value if you know what you are doing!!!
        /// </summary>
        protected TStartArguments startArguments;
    }
}
