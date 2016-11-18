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
			return File.Open(absoluteFilePath, mode, access);
		}

		protected override bool IsDirectory(string path)
		{
			var att = File.GetAttributes(GetFullURL(path));
			return (att & FileAttributes.Directory) == FileAttributes.Directory;
		}
	}
}

#endif