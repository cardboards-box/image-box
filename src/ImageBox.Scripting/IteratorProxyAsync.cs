namespace ImageBox.Scripting;

/// <summary>
/// Provides a proxy for iterating through collections in box-scripts
/// </summary>
public class IteratorProxyAsync(
	IAsyncEnumerator<dynamic> _enumerator)
{
	/// <summary>
	/// The current item in the enumerator
	/// </summary>
	[ModuleExport(type: typeof(JsValue))]
	public dynamic Current => _enumerator.Current;

	/// <summary>
	/// Creates an instance of the iterator proxy from an async enumerator
	/// </summary>
	/// <param name="collection">The collection of items</param>
	public IteratorProxyAsync(IAsyncEnumerable<dynamic> collection)
		: this(collection.GetAsyncEnumerator()) { }

	/// <summary>
	/// Moves to the next item in the iterator
	/// </summary>
	/// <returns>true if the enumerator moved to the next item</returns>
	[ModuleExport]
	public ValueTask<bool> MoveNext()
	{
		return _enumerator.MoveNextAsync();
	}

	/// <summary>
	/// Gets the next item in the iterator or null if the end is reached
	/// </summary>
	/// <returns>The next item in the iterator or null if there is no next item</returns>
	[ModuleExport(type: typeof(Task<JsValue>))]
	public async ValueTask<dynamic?> Next()
	{
		if (!await MoveNext()) return null;

		return Current;
	}

	/// <summary>
	/// Disposes of the underlying iterator.
	/// </summary>
	[ModuleExport]
	public async ValueTask Dispose()
	{
		await _enumerator.DisposeAsync();
	}
}
