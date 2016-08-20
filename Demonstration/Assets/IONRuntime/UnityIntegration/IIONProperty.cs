using ION.Core;
using ION.Core.Extensions;

public interface IIONProperty
{
	IEntityProperty IONProperty { get ; }
	
	// Henrique Campos - changed initialization from the Start() in order to solve random problems regarding processing times
	// e.g., sometimes the Property initialization is faster than the Entity's. Thus, causing some random errors.
	// Now it is the IONEntity who calls the function.
	void Initialize();
}

