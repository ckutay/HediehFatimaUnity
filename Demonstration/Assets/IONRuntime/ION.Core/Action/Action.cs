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
//  05/05/2009      Guilherme Raimundo <guilherme.raimundo@inesc-id.pt>
//  Changed Step Request scheduling method to pass through the process Events phase.
//  Stepped Event -> New Step Request | Started Event -> New Step Request | Resumed Event -> New Step Request
//  ---
//  29/05/2009      Guilherme Raimundo <guilherme.raimundo@inesc-id.pt>
//  - Solved an error of Events of state changed being raised before the actual change took place. 
//  This error only occured if the Action was outside the Simulation. I.E. when changes were made assynchronously.
//  - Renamed enum typed Action.State to ActionState and Moved it to ION.Core Namespace.
//  I.E. it is no longer nested in Action class.  

using System;
using System.Linq;
using ION.Core.Events;
using ION.Meta;
using System.Collections.Generic;

namespace ION.Core
{
    public class Action : Element, IAction
    {
        /// <summary>
        /// An Action is an Element that represents an ongoing operation.
        /// During this process it can be in three possible states: Running, Idle or Paused.
        /// </summary>
        public Action() 
        {
            this.state = ActionState.Idle;

            this.RequestHandlers.Set(this.HandleRequests, this.HandledTypes);
            this.EventHandlers.Add<IStepped>(this.OnStep);
            this.EventHandlers.Add<IStarted>(this.OnStart);
            this.EventHandlers.Add<IResumed>(this.OnResume);

            this.InitializeGenericEventInformation();
        }

        private void InitializeGenericEventInformation()
        {
            this.EventTypeParams[0] = this.GetType();
            this.EventTypeArguments[0] = this.GetType();
            this.EventTypeArguments[1] = typeof(ActionState);
            this.EventChangedStateArguments[0] = this.EventSteppedArguments[0] = this;
        }

        internal virtual Type[] HandledTypes
        {
            get
            {
                return new Type[] {typeof (StartRequest), typeof (StopSuccessRequest),
                                   typeof (StopFailRequest), typeof (PauseRequest),
                                   typeof (ResumeRequest), typeof (StepRequest)};
            }
        }
 
        #region Requests
        internal protected sealed class StartRequest : Request {}
        internal protected sealed class ResumeRequest : Request {}
        internal protected sealed class StopSuccessRequest : Request {}
        internal protected sealed class StopFailRequest : Request {}
        internal protected sealed class PauseRequest : Request {}
        internal protected sealed class StepRequest : Request {}
        #endregion 

        #region Request Handlers

        /// <summary>
        /// Calls the appropriate handler according to the state of the action.
        /// Has the following policy:
        ///     Idle State Request Priorities:
        ///         Start > Other requests
        ///     Running State Request Priorities:
        ///         Stop Fail > Stop Sucess > Pause > Step > Other Requests
        ///     Paused State Request Priorities:
        ///         Stop Fail > Stop Success > Resume > Other Requests
        /// </summary>
        protected virtual void HandleRequests(IEnumerable<Request> requests)
        {
            switch (this.state)
            {
                case ActionState.Idle:
                    ExecuteIdleState(requests.OfType<StartRequest>().ToArray());
                    break;
                case ActionState.Running:
                    ExecuteRunningState(requests.OfType<StopSuccessRequest>().ToArray(), requests.OfType<StopFailRequest>().ToArray(), requests.OfType<PauseRequest>().ToArray(), requests.OfType<StepRequest>().ToArray());
                    break;
                case ActionState.Paused:
                    ExecutePausedState(requests.OfType<StopSuccessRequest>().ToArray(), requests.OfType<StopFailRequest>().ToArray(), requests.OfType<ResumeRequest>().ToArray());
                    break;
                default:
                    throw new Exception("Unknown State Type: " + this.state);
            }
        }

        private void ExecuteIdleState(StartRequest[] Start)
        {
            if (Start.Length > 0)
            {
                this.ExecuteStart();
            }
        }

        internal void ExecuteRunningState(StopSuccessRequest[] StopSucceed,
            StopFailRequest[] StopFail, PauseRequest[] Pause, StepRequest[] stepRequests)
        {
            if (StopFail.Length > 0)
            {
                this.ExecuteStopFail();
            }
            else if (StopSucceed.Length > 0)
            {
                this.ExecuteStopSuccess();
            }
            else if (Pause.Length > 0)
            {
                this.ExecutePause();
            }
            else if(stepRequests.Length > 0)
            {
                this.ExecuteStep();
            }
        }

        internal void ExecutePausedState(StopSuccessRequest[] StopSucceed,
            StopFailRequest[] StopFail,
            ResumeRequest[] Resume)
        {
            if (StopFail.Length > 0)
            {
                this.ExecuteStopFail();
            }
            else if (StopSucceed.Length > 0)
            {
                this.ExecuteStopSuccess();
            }
            else if (Resume.Length > 0)
            {
                this.ExecuteResume();
            }
        }

        internal void ExecuteStart()
        {
            this.EventChangedStateArguments[1] = this.state;
            this.state = ActionState.Running;
            this.Raise(Event.New(typeof(Started<>), this.EventTypeParams, this.EventTypeArguments, this.EventChangedStateArguments));
        }

        private void ExecuteStopSuccess()
        {
            this.EventChangedStateArguments[1] = this.state;
            this.state = ActionState.Idle;
            this.Raise(Event.New(typeof (Succeeded<>), this.EventTypeParams, this.EventTypeArguments,
                                     this.EventChangedStateArguments));
        }

        private void ExecuteStopFail()
        {
            this.EventChangedStateArguments[1] = this.state;
            this.state = ActionState.Idle;
            this.Raise(Event.New(typeof(Failed<>), this.EventTypeParams, this.EventTypeArguments, this.EventChangedStateArguments));
        }


        private void ExecutePause()
        {
            this.EventChangedStateArguments[1] = this.state;
            this.state = ActionState.Paused;
            this.Raise(Event.New(typeof(Paused<>), this.EventTypeParams, this.EventTypeArguments, this.EventChangedStateArguments));
        }

        private void ExecuteResume()
        {
            this.EventChangedStateArguments[1] = this.state;
            this.state = ActionState.Running;
            this.Raise(Event.New(typeof(Resumed<>), this.EventTypeParams, this.EventTypeArguments, this.EventChangedStateArguments));
        }

        private void ExecuteStep()
        {
            this.Raise(Event.New(typeof(Stepped<>), this.EventTypeParams, this.EventSteppedArguments));
        }

        #endregion

        #region Event Handlers
        protected void OnStep(IStepped evt)
        {
            if(evt.Action == this)
            {
                this.Schedule(new StepRequest());
            }
        }

        protected void OnStart(IStarted evt)
        {
            if (evt.Action == this)
            {
                this.Schedule(new StepRequest());
            }
        }

        protected void OnResume(IResumed evt)
        {
            if (evt.Action == this)
            {
                this.Schedule(new StepRequest());
            }
        }

        #endregion

        #region Action Members

        /// <summary>
        /// Starts the Action.
        /// </summary>
        public void Start()
        {
            this.Schedule(new StartRequest());
        }

        /// <summary>
        /// Stops the Action.
        /// </summary>
        /// <param name="success">
        /// Indicates whether the Action stops successfully or not.
        /// </param>
        public void Stop(bool success)
        {
            if(success)
                this.Schedule(new StopSuccessRequest());
            else
                this.Schedule(new StopFailRequest());
        }

        /// <summary>
        /// Pauses the Action.
        /// </summary>
        public void Pause()
        {
            this.Schedule(new PauseRequest());
        }

        /// <summary>
        /// Resumes the Action.
        /// </summary>
        public void Resume()
        {
            this.Schedule(new ResumeRequest());
        }

        /// <summary>
        /// Gets the Action current State.
        /// </summary>
        public ActionState CurrentState
        {
            get { return this.state; }
        }

        /// <summary>
        /// Direcly changes the value of the Action state without regarding the synchronization cycle. 
        /// Only change this value if you know what you are doing!!!
        /// </summary>
        protected ActionState state; 

        #endregion

        public override void OnDestroy() {}

        #region Event Generic Info
        internal readonly Type[] EventTypeParams = new Type[1];
        internal readonly Type[] EventTypeArguments = new Type[2];
        internal readonly object[] EventChangedStateArguments = new object[2];
        private readonly object[] EventSteppedArguments = new object[1];
        #endregion
    }


}
