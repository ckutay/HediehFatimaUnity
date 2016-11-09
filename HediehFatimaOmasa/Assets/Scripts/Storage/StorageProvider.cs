using GAIPS.Rage;

namespace Assets.Scripts
{
	public sealed class StorageProvider
	{
		private static readonly IStorageProvider m_provider = BuildStorageProvider();
		public static IStorageProvider CurrentProvider
		{
			get { return m_provider; }
		}

		private static IStorageProvider BuildStorageProvider()
		{
#if UNITY_WEBGL && !UNITY_EDITOR
			return new WebGLStorageProvider();
#else
			return new StreamingAssetsStorageProvider();
#endif
		}
	}
}