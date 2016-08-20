using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using ION.Meta;

namespace ION.Core.Extensions
{
	public class Entity : Element
	{
		private static Dictionary<string,ulong> name2id = new Dictionary<string,ulong> ();

		public static ulong getIDByName (string name)
		{
			return name2id [name];
		}

		public string Name { get; private set; }

		private Dictionary<string,IEntityProperty> properties;
		private Dictionary<string,IEntityAction> actions;
		private Simulation sim;

		public Entity (string name)
		{
			this.Name = name;
			// Henrique Campos - changed this, so that elements can be removed and added again on the simulation
			if (name2id.ContainsKey (name))
				name2id [name] = this.UID;
			else
				name2id.Add (name, this.UID);
            
			this.sim = null;
			this.properties = new Dictionary<string, IEntityProperty> ();
			this.actions = new Dictionary<string,IEntityAction> ();
		}

		public void AddProperty (string visibility, IEntityProperty p)
		{
			//UnityEngine.Debug.Log(p.Name);
			if (properties.ContainsKey (p.Name))
				properties[p.Name]=p;
			else{
			this.properties.Add (p.Name, p);
				
			if (this.sim != null) {
				this.sim.Elements.Add (p as Element);

				PropertyAdded evt = new PropertyAdded (visibility, p, this);

				//ApplicationLogger.Instance().WriteLine("Adding a new property  and generating the corresponding event: " + this.Name + " " + p.Name + " " + p.Value);
				// raise event Added<>
				this.Raise (evt);
			}
			}
			
		}

		public void AddProperty (IEntityProperty p)
		{
			AddProperty ("*", p);
		}

		public void AddAction (IEntityAction a)
		{
           
			//UnityEngine.Debug.Log(a.ToString());
			this.actions.Add (a.Name, a);
			if (this.sim != null) {
				this.sim.Elements.Add (a as Element);
			}
			
		}

		public IEnumerable<IEntityProperty>  getProperties ()
		{
			return this.properties.Values;
		}

		public string getStringWithAllProperties ()
		{
			string msg = "";
			foreach (IEntityProperty p in this.properties.Values) {
				msg += p.Name + ":" + p.Value.ToString () + " ";
			}

			return msg;
		}

		public IEntityAction getActionByName (string name)
		{
			if (this.actions.ContainsKey (name)) {
				return this.actions [name];
			} else
				return null;
		}

		public IEntityProperty getPropertyByName (string name)
		{
			if (this.properties.ContainsKey (name)) {
				return this.properties [name];
			} else
				return null;
		}

		public void SetProperty<T> (string visibility, string name, T value)
		{
			EntityProperty<T> p;

			if (this.properties.ContainsKey (name)) {
				p = this.properties [name] as EntityProperty<T>;
				p.SetRestrictedValue (visibility, value);
			} else {
				p = new EntityProperty<T> (this, name, value);
				this.AddProperty (visibility, p);
			}
		}

		public virtual void AddToSimulation (Simulation s)
		{
			this.sim = s;
			foreach (IEntityProperty p in this.properties.Values) {
				s.Elements.Add (p as Element);
			}

			foreach (IEntityAction a in this.actions.Values) {
				s.Elements.Add (a as Element);
			}

			s.Elements.Add (this);
		}

		public override void OnDestroy ()
		{
			name2id.Remove (Name);
		}

	}
}
