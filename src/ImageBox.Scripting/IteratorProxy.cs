namespace ImageBox.Scripting;

/// <summary>
/// Provides a proxy for iterating through collections in box-scripts
/// </summary>
public class IteratorProxy(
	IEnumerator<dynamic> _enumerator) : IDisposable
{
	/// <summary>
	/// The current item in the enumerator
	/// </summary>
	[ModuleExport(type: typeof(JsValue))]
	public dynamic Current => _enumerator.Current;

	/// <summary>
	/// Provides a proxy for iterating through collections in box-scripts
	/// </summary>
	/// <param name="enumerable">The enumerable to use as the source</param>
	public IteratorProxy(IEnumerable<dynamic> enumerable)
		: this(enumerable.GetEnumerator()) { }

	/// <summary>
	/// Moves to the next item in the iterator
	/// </summary>
	/// <returns>true if the enumerator moved to the next item</returns>
	[ModuleExport]
	public bool MoveNext()
	{
		return _enumerator.MoveNext();
	}

	/// <summary>
	/// Gets the next item in the iterator or null if the end is reached
	/// </summary>
	/// <returns>The next item in the iterator or null if there is no next item</returns>
	[ModuleExport(type: typeof(JsValue))]
	public dynamic? Next()
	{
		if (!MoveNext()) return null;

		return Current;
	}

	/// <summary>
	/// Resets the enumerator to the initial position
	/// </summary>
	[ModuleExport]
	public void Reset() => _enumerator.Reset();

	/// <summary>
	/// Disposes of the underlying iterator.
	/// </summary>
	[ModuleExport]
	public void Dispose()
	{
		_enumerator.Dispose();
		GC.SuppressFinalize(this);
	}
}
