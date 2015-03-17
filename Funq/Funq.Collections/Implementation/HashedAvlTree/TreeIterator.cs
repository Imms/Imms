using System.Collections.Generic;
using Funq.Collections.Common;

namespace Funq.Collections.Implementation
{
	internal partial class HashedAvlTree<TKey, TValue>
	{
		public class TreeIterator
		{
			private readonly List<Marked<Node, bool>> _future;
			private Node _current;
			public TreeIterator(int maxHeight)
			{
				_future = new List<Marked<Node, bool>>(maxHeight);
			}

			public TreeIterator(Node root)
				: this(root.MaxPossibleHeight)
			{
				_future.Add(Marked.Create(root, false));
			}

			public bool MoveNext()
			{
				while (_future.Count > 0)
				{
					var cur =  _future.PopLast();
					if (cur.Mark) return SetCurrent(cur);
					var node = cur.Object;
					if (node.IsNull) continue;
					if (!node.Right.IsNull) _future.Add(node.Right.Mark(false));			
					_future.Add(node.Mark(true));
					if (!node.Left.IsNull) _future.Add(node.Left.Mark(false));
				}
				return false;
			}

			public bool SeekGreaterThan(int hash)
			{
				var isEnded = SeekForwardCloseTo(hash);
				if (!isEnded) return false;
				if (_current.Hash >= hash) return true;
				return this.MoveNext();
			}

			public bool IsEnded
			{
				get
				{
					return _future.Count == 0;
				}
			}

			public Node Current
			{
				get
				{
					return _current;
				}
			}

			private bool SetCurrent(Node node)
			{
				_current = node;
				return true;
			}

			/// <summary>
			/// Skips until it reaches 1 after or 1 before the hash. Worst case O(logn), amortized O(1).
			/// </summary>
			/// <param name="hash"></param>
			/// <returns></returns>
			private bool SeekForwardCloseTo(int hash)
			{
				//If we're already at the desired node, return true.
				if (_current != null && _current.Hash >= hash) return true;
				//Climb up until the current node is larger than the hash or until the root is reached.
				while (_future.Count > 1)
				{
					var cur = _future.PopLast();
					//We ignore all nodes other than parents we've already passed.
					if (!cur.Mark) continue;
					//If we haven't found the right node, we stop here:
					if (cur.Object.Hash >= hash)
					{
						_future.Add(cur);
						break;
					}
				}
				
				//Now we climb down again, in order to find the node in question.
				while (_future.Count > 0)
				{
					var cur = _future.PopLast();
					var node = cur.Object;
					if (node.IsNull && _future.Count > 0) return SetCurrent(_future.LastItem());
					if (node.IsNull) return false;
					if (cur.Mark && node.Hash != hash) continue;
					switch (hash > node.Hash ? Cmp.Greater : hash < node.Hash ? Cmp.Lesser : Cmp.Equal)
					{
						case Cmp.Greater:
							if (node.Right.IsNull) return SetCurrent(node);
							_future.Add(node.Right.Mark(false));
							break;
						case Cmp.Lesser:
							if (!node.Right.IsNull) _future.Add(node.Right.Mark(false));
							if (node.Left.IsNull) return SetCurrent(node);
							_future.Add(node.Mark(true));
							_future.Add(node.Left.Mark(false));
							break;
						case Cmp.Equal:
							if (!cur.Mark && !node.Right.IsNull) _future.Add(node.Right.Mark(false));
							return SetCurrent(node);
					}
				}
				return false;
			}
		}
	}
}
