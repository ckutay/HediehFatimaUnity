using System.Linq;
using System.Collections.Generic;
using ION.Meta;
using ION.Core;
using ION.SyncCollections;

using ION.Core.Extensions;

using UnityEngine;


public class IONEntity : MonoBehaviour
{
	public string entityName;
	protected Entity entity;
	private bool initialized = false;
	
	public bool Initialized {
		get
		{
			return this.initialized;
		}
	}

	public Entity Entity
	{
		get
		{
			return this.entity;
		}
	}
		
	private void Start ()
	{
		initialized = false;
	}
	
	public void Update()
	{
		//late initialization
		if(!this.initialized)
		{	
			this.entity = new Entity(entityName);
			
			Debug.Log("IONEntity initialization: " + this.entity.Name);
			
			Component[] actionComponents = this.GetComponents (typeof(ICharacterAction));
			foreach (ICharacterAction actionComponent in actionComponents)
			{
				actionComponent.Initialize(); // Henrique Campos - added this regarding new initialization process
				
				Debug.Log("Action " + actionComponent.Action.Name + " added to Entity " + this.entity);
				this.entity.AddAction(actionComponent.Action);
			}
			
			Component[] propertyComponents = this.GetComponents(typeof(IIONProperty));
			foreach (IIONProperty property in propertyComponents)
			{
				property.Initialize(); // Henrique Campos - added this regarding new initialization process
				
				Debug.Log("Property " + property.IONProperty.Name + " added to Entity " + this.entity);
				this.entity.AddProperty(property.IONProperty); 
			}
			
			
			this.entity.AddToSimulation(Simulation.Instance);
			
			//Henrique Campos - moved this instruction from the start of this block to the end of it
			this.initialized = true;
		}
	}
	
	public bool HasProperty(string name)
	{
		return this.Entity.getPropertyByName(name) != null;
	}
	
 	public T GetPropertyValue<T>(string name)
 	{
		EntityProperty<T> p = this.Entity.getPropertyByName(name) as EntityProperty<T>;
		if(p!=null)
		{
			return p.Value;
		}
		else throw new PropertyUnavailableException(this.name,name);
 	}

    public void SetPropertyValue<T>(string name, T value)
    {
		this.Entity.SetProperty<T>("*",name,value);
       
    }
	
	protected class PropertyUnavailableException : UnityException
	{
		public PropertyUnavailableException(string entity, string property)
		: base("Property " + property + " in Entity " + entity + " is not available") {}
	}
}