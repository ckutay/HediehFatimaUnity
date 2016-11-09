#if !UNITY_WEBGL || UNITY_EDITOR

using System.IO;
using GAIPS.Rage;
using UnityEngine;

namespace Assets.Scripts
{
	public class StreamingAssetsStorageProvider : BaseStorageProvider
	{
		public StreamingAssetsStorageProvider() : base(Application.streamingAssetsPath){}

		protected override Stream LoadFile(string absoluteFilePath, FileMode mode, FileAccess access)
		{
			//Debug.LogWarning(absoluteFilePath);

			return File.Open(absoluteFilePath, mode, access);
		}

		protected override bool IsDirectory(string path)
		{
			//Debug.LogWarning(path);
			var att = File.GetAttributes(GetFullURL(path));
			return (att & FileAttributes.Directory) == FileAttributes.Directory;
		}
	}
}

#endif