

using ION.Meta;
using ION.Core.Events;

namespace ION.Core.Extensions.Events
{
    /// <remarks>
    /// Indicates that a Property has changed its value.
    /// </remarks>
    public interface IRestrictedPropertyChange : IEvent
    {
        string Visibility { get; }

        Entity Entity { get; }

        IEntityProperty Property { get; }

        object OldValue { get; }

        object NewValue { get; }
		
		string ToXml();
    }


    public interface IRestrictedPropertyChange<TProperty> : IValueChanged<TProperty>
    {
        string Visibility { get; }
    }


    /// <remarks>
    /// Indicates that a Property has changed its value.
    /// </remarks>
    public interface IRestrictedPropertyChange<TOldValue, TNewValue> : IValueChanged<TOldValue,TNewValue>
    {
        string Visibility { get; }
    }

    /// <remarks>
    /// Indicates that a Property has changed its value.
    /// </remarks>
    public interface IRestrictedPropertyChange<TOldValue, TNewValue, TProperty> : IRestrictedPropertyChange<TOldValue, TNewValue>, IRestrictedPropertyChange<TProperty>
        where TProperty : IEntityProperty 
    {
    }
}


