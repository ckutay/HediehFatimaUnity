using System.Linq;
using System.Collections.Generic;
using ION.Meta;
using ION.Core;
using ION.SyncCollections;

using ION.Core.Extensions;
using ION.Core.Extensions.Events;

using UnityEngine;

[RequireComponent (typeof(IONEntity))]
public class IONBooleanProperty : MonoBehaviour, IIONProperty
{
	private EntityProperty<bool> property;
	
	public string name;
	public bool propertyValue;
	private bool oldValue;
	
	// Henrique Campos - added this regarding new initialization process
	private bool initialized = false;
	
	public IEntityProperty IONProperty
	{
		get
		{
			return this.property;
		}
	}
	
	public ION.Core.Property.SetValuePolicy SetValuePolicy
	{
		get
		{
			return this.property.SetValuePolicy;
		}
	}
	
	
	private void Start ()
	{
		// Henrique Campos - added this regarding new initialization process
		initialized = false;
	}
	
	
	// Henrique Campos - changed initialization from the Start() in order to solve random problems regarding processing times
	// e.g., sometimes the Property initialization is faster than the Entity's. Thus, causing some random errors.
	public void Initialize(){
		
		this.property = new EntityProperty<bool>(this.GetComponent<IONEntity>().Entity,this.name,this.propertyValue);
		this.property.EventHandlers.Add<IRestrictedPropertyChange>(this.OnChange);
		this.oldValue = propertyValue;
		initialized = true;
		return;
	}
	
	public void Update()
	{
		if(!initialized){
			// Henrique Campos - added this regarding new initialization process
			return;
		}
		
		if(this.propertyValue != oldValue)
		{
			this.GetComponent<IONEntity>().Entity.SetProperty<bool>("*",this.name,this.propertyValue);
			this.oldValue = this.propertyValue;
		}
	}
		   
		   
	public void OnChange(IRestrictedPropertyChange evt)
	{
		this.propertyValue = (bool) evt.NewValue;
		this.oldValue = this.propertyValue;
	}
		   
}