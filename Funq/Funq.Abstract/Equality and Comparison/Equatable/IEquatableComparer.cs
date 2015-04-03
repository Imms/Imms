﻿using System;
using System.Collections.Generic;

namespace Funq.Abstract {
	/// <summary>
	/// Guarantees that this comparer implements functional equality.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IEquatableComparer<T> : IComparer<T>, IEquatable<IComparer<T>>
	{
		
	}
}