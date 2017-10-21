using AssetPackage;
using UnityEngine;
//using ILogger = GAIPS.Rage.ILogger;

namespace Assets.Scripts
{
	public class AssetManagerBridge : IBridge//, ILogger
	{
		public void Log(object msg)
		{
			Debug.Log(msg);
		}
	}
}