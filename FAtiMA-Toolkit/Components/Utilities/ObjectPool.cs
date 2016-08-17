﻿namespace Utilities
{
	/// <summary>
	/// Generic Object Pool.
	/// Used to minimize memory allocations by recycling objects
	/// </summary>
	/// <typeparam name="T">The object type that is being stored in the pool.</typeparam>
	public static class ObjectPool<T> where T: class, new()
	{
		private static PoolNode _root = null;

		/// <summary>
		/// The amount of Objects instantiated by the pool
		/// </summary>
		public static uint GeneratedAmount
		{
			get;
			private set;
		}

		static ObjectPool()
		{
			GeneratedAmount = 0;
		}

		/// <summary>
		/// Returns a recycled object instance.
		/// If there is no objects inside the pool, a new one is created.
		/// </summary>
		public static T GetObject()
		{
			if (_root == null)
			{
				GeneratedAmount++;
				return new T();
			}

			PoolNode node = _root;
			_root = node.Next;

			var value = (T)node.Value;
			node.Value = null;
			PoolNode.Recycle(node);

			return value;
		}

		/// <summary>
		/// Send the given object to the recycle pool.
		/// </summary>
		/// <param name="value"></param>
		public static void Recycle(T value)
		{
			PoolNode node = PoolNode.GetNewNode();
			node.Value = value;
			node.Next = _root;
			_root = node;
		}

		/// <summary>
		/// Gets the pool's current capacity
		/// </summary>
		public static uint Count()
		{
			uint count = 0;
			PoolNode it = _root;
			while (it != null)
			{
				count++;
				it = it.Next;
			}
			return count;
		}
	}

	/// <summary>
	/// Helper class used by the ObjectPool classes.
	/// </summary>
	internal sealed class PoolNode
	{
		private static PoolNode _root = null;

		public static PoolNode GetNewNode()
		{
			if (_root == null)
				return new PoolNode();

			var n = _root;
			_root = _root.Next;
			n.Next = null;
			n.Value = null;
			return n;
		}

		public static void Recycle(PoolNode node)
		{
			node.Value = null;
			node.Next = _root;
			_root = node;
		}

		public PoolNode Next;
		public object Value;
	}
}
