using UnityEngine;
using System.Collections;
using ION.Core;
using ION.Core.Events;
using ION.Core.Extensions;
using ION.Meta;
using System.Collections.Generic;

[RequireComponent (typeof(IONEntity))]
[RequireComponent (typeof(UnityController))]
public abstract class IONAction : MonoBehaviour, ICharacterAction
{	
	protected IONEntity _thisEntity;
	protected EntityAction<ActionParameters> _action;
	
	protected bool _initialized = false;
	protected UnityController UnityController { get; private set; }
	
	IEntityAction ICharacterAction.Action { get { return this._action; } }
	public EntityAction<ActionParameters> Action { get { return this._action; } }
	
	protected abstract string ActionName { get; }
	
	private bool _waitingToStop; // -- Determines if an Action Stop was issued
	private bool _sucessfulStop; // -- Determines if the Action was sucessful or failed
	private List<IPropertyChangedByAction> _propertiesToWaitChange; // -- Contains the Properties that were changed by this Action
				
	private void Awake()
	{
		_thisEntity = this.GetComponent<IONEntity>();
	}
	
	private void Start() 
	{
		_initialized = false;
		this.UnityController = this.GetComponent<UnityController>();
		_waitingToStop = false;
		_sucessfulStop = true;
		_propertiesToWaitChange = new List<IPropertyChangedByAction>();
	}
		
	public void Initialize(){
		_action = new EntityAction<ActionParameters>(_thisEntity.Entity, ActionName);
		_action.EventHandlers.Add<IStarted<EntityAction<ActionParameters>>>(this.OnStart);
		_action.EventHandlers.Add<IStepped<EntityAction<ActionParameters>>>(this.CheckStep);
		_action.EventHandlers.Add<ISucceeded<EntityAction<ActionParameters>>>(this.OnSucceeded);
		_action.EventHandlers.Add<IFailed<EntityAction<ActionParameters>>>(this.OnFailed);
		_action.EventHandlers.Add<IStopped<EntityAction<ActionParameters>>>(this.OnStop);
		_initialized = true;
	}
	
	// -- Event handler for Action's steps. Checks if an Action Stop was issued. 
	// ----- If not, performs an OnStep. 
	// ----- If the the Action Stop was issued, checks if Properties were sucessfully changed before proceeding to Stop.
	private void CheckStep(IStepped<EntityAction<ActionParameters>> steppedEvt){
		if(!_waitingToStop){
			OnStep(steppedEvt);
		}
		else {
			if(CheckIfPropertiesChanged()){
				_waitingToStop = false;
				this.Action.Stop(_sucessfulStop);
			}
		}
	}
	
	// -- Issues an Action Stop
	protected void ActionStop(bool sucess){
		_sucessfulStop = sucess;
		_waitingToStop = true;
	}
	
	// -- Debug purposes only
	protected void ActionLog(string msg){
		Debug.Log("[IONAction]" + msg);
	}
	
	#region Event Handlers to be defined by the ION Actions
	public virtual void OnStart(IStarted<EntityAction<ActionParameters>> startEvt){}
	
	public virtual void OnStep(IStepped<EntityAction<ActionParameters>> steppedEvt){
		ActionStop(true);
	}
	
	public virtual void OnSucceeded(ISucceeded<EntityAction<ActionParameters>> succeededEvt){}
	
	public virtual void OnFailed(IFailed<EntityAction<ActionParameters>> failedEvt){}
	
	public virtual void OnStop(IStopped<EntityAction<ActionParameters>> stoppedEvt){}
	#endregion
	
	#region Property changed by ION Actions related methods and classes
	// -- Sets a property of an entity with a certain new value. This property is added to the list of property to be checked before the ION Action stops
	protected void SetActionEffect<T>(IONEntity entity, string pName, T pNewValue){
		PropertyChangedByAction<T> pChanged;
		
		if(entity.HasProperty(pName)){
			T pOldValue = entity.GetPropertyValue<T>(pName);
			pChanged = new PropertyChangedByAction<T>(entity, pName, pOldValue, pNewValue);
		}
		else {
			pChanged = new PropertyChangedByAction<T>(entity, pName, pNewValue);
		}
		
		_propertiesToWaitChange.Add(pChanged);
		//PropertyChangedByAction<T> pChanged = new PropertyChangedByAction<T>();
		
		entity.SetPropertyValue<T>(pName, pNewValue);
	}
	
	// -- Overloaded method for changing a property in this ION Action's Entity
	protected void SetActionEffect<T>(string pName, T pNewValue){
		SetActionEffect<T>(_thisEntity, pName, pNewValue);
	}
	
	// -- Checks if all properties issued to be changed by the ION Action have already been changed in the ION simulation
	protected bool CheckIfPropertiesChanged(){
		if(_propertiesToWaitChange.Count == 0){
			return true;
		}
		else {
			foreach(IPropertyChangedByAction pChanged in _propertiesToWaitChange){
				if(!pChanged.CheckIfChanged()){
					return false;
				}
			}
			
			_propertiesToWaitChange.Clear();
			return true;
		}
	}
	
	// -- Interface for class that defines a property change by an ION Action
	protected interface IPropertyChangedByAction {
		IONEntity PropertyEntity { get; }
		string PropertyName { get; }
		
		bool CheckIfChanged();
	}
	
	// -- Class that defines the information prior to a property change by an ION Action
	protected class PropertyChangedByAction<T> : IPropertyChangedByAction {
		public IONEntity PropertyEntity { get; private set; }
		public string PropertyName { get; private set; }
			
		private bool NewProperty { get; set; }
		private T OldValue { get; set; }
		private T NewValue { get; set; }
	
		// -- This constructor is used when the property does not exist yet, so there is no old value.
		public PropertyChangedByAction (IONEntity entity, string pName, T newValue){
			PropertyEntity = entity;
			PropertyName = pName;
			
			NewProperty = true;
			
			OldValue = default(T);
			NewValue = newValue;
		}
		
		// -- This constructor is used when the property already exist and there is an old value specified.
		public PropertyChangedByAction (IONEntity entity, string pName, T oldValue, T newValue){
			PropertyEntity = entity;
			PropertyName = pName;
			
			NewProperty = false;
			
			OldValue = oldValue;
			NewValue = newValue;
		}
		
		// -- Checks if the property has changed accordingly
		public bool CheckIfChanged (){
			if(PropertyEntity.HasProperty(PropertyName)){
				T pCurrValue = PropertyEntity.GetPropertyValue<T>(PropertyName);
				
				if(pCurrValue.Equals(NewValue)){ // -- Check if the current value of the property matches the expected value to be changed
					return true;
				}
				else if(NewProperty){ // -- If the value of the created property does not match the value expected, something went wrong in the simulation ..
					throw new IncoherentPropertyChange(PropertyEntity.entityName, PropertyName, NewValue.ToString(), pCurrValue.ToString());
				}
				else if(!pCurrValue.Equals(OldValue)){ // -- If the property's old value does match the one specified before the change, something went wrong in the simulation ..
					throw new IncoherentPropertyChange(PropertyEntity.entityName, PropertyName, OldValue.ToString(), NewValue.ToString(), pCurrValue.ToString());
				}
			}
			
			return false;
		}
	}
	
	// -- Exception used when a property enters an 'incoherent' state (expected values do not match)
	public class IncoherentPropertyChange : UnityException {
		
		// -- Exception constructor used for new properties
		public IncoherentPropertyChange(string entity, string property, string newValue, string actualValue) : 
			base(string.Format("Incoherent property state. Property {0} in entity {1} should be new and value should be {2}. However, it is {3}.",
				property, entity, newValue, actualValue))
		{}
		
		// -- Exception constructor used for properties that already exist and have a previous value
		public IncoherentPropertyChange(string entity, string property, string oldValue, string newValue, string actualValue) : 
			base(string.Format("Incoherent property state. Property {0} in entity {1} old value should be {2} and new value should be {3}. However, it is {4}.",
				property, entity, oldValue, newValue, actualValue))
		{}
	}
	#endregion
}
