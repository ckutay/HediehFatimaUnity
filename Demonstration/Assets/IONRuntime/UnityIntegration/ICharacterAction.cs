using ION.Core;
using ION.Core.Extensions;

public interface ICharacterAction
{
	IEntityAction Action { get ; }
	void Initialize();
}


