using System.Linq;
using System.Collections.Generic;
using ION.Meta;
using ION.Core;
using ION.SyncCollections;

using ION.Core.Extensions;
using ION.Core.Extensions.Events;

using UnityEngine;

[RequireComponent (typeof(IONEntity))]
public class IONStringProperty : MonoBehaviour, IIONProperty
{
	private EntityProperty<string> property;
	
	public string name;
	public string propertyValue;
	private string oldValue;
	
	// Henrique Campos - added this regarding new initialization process
	private bool initialized = false;
	
	public IEntityProperty IONProperty
	{
		get
		{
			return this.property;
		}
	}
	
	private void Start ()
	{
		// Henrique Campos - added this regarding new initialization process
		initialized = false;
	}
	
	// Henrique Campos - changed initialization from the Start() in order to solve random problems regarding processing times
	// e.g., sometimes the Property initialization is faster than the Entity's. Thus, causing some random errors.
	// Now it is the IONEntity who calls the function.
	public void Initialize(){		
		this.property = new EntityProperty<string>(this.GetComponent<IONEntity>().Entity,this.name,this.propertyValue);
		this.property.EventHandlers.Add<IRestrictedPropertyChange>(this.OnChange);
		this.oldValue = propertyValue;
		initialized = true;
	}
	
	public void Update()
	{
		// Henrique Campos - added this regarding new initialization process
		if(!initialized)
			return;
		
		if(this.propertyValue != oldValue)
		{
			this.GetComponent<IONEntity>().Entity.SetProperty<string>("*",this.name,this.propertyValue);
			this.oldValue = this.propertyValue;
		}
	}
	
	public void OnChange(IRestrictedPropertyChange evt)
	{
		this.propertyValue = (string) evt.NewValue;
		this.oldValue = this.propertyValue;
	}
}
