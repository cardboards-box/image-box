﻿namespace ImageBox.Services;

using Ast;

/// <summary>
/// An exception thrown if there is an error rendering or loading the boxed image
/// </summary>
/// <param name="Message">The exception message</param>
/// <param name="Image">The image that caused the exception</param>
/// <param name="InnerException">Any inner exceptions</param>
/// <param name="Context">The AST element context</param>
public class RenderContextException(
    string Message,
    LoadedAst? Image = null,
    Exception? InnerException = null,
    AstElement?[]? Context = null) : Exception(Message, InnerException)
{
    /// <summary>
    /// An exception thrown if there is an error rendering or loading the boxed image
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="image">The image that caused the exception</param>
    /// <param name="Context">The AST element context</param>
    public RenderContextException(string message, LoadedAst image, params AstElement?[] Context)
        : this(message, image, null, Context) { }

    /// <summary>
    /// An exception thrown if there is an error rendering or loading the boxed image
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="inner">The inner exception that caused all of this</param>
    /// <param name="image">The image that caused the exception</param>
    /// <param name="Context">The AST element context</param>
    public RenderContextException(string message, Exception inner, LoadedAst image, params AstElement?[] Context)
        : this(message, image, inner, Context) { }


    /// <summary>
    /// An exception thrown if there is an error rendering or loading the boxed image
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="Context">The AST element context</param>
    public RenderContextException(string message, params AstElement?[] Context)
        : this(message, (LoadedAst?)null, null, Context) { }

    /// <summary>
    /// An exception thrown if there is an error rendering or loading the boxed image
    /// </summary>
    /// <param name="message">The exception message</param>
    /// <param name="inner">The inner exception that caused all of this</param>
    /// <param name="Context">The AST element context</param>
    public RenderContextException(string message, Exception inner, params AstElement?[] Context)
        : this(message, null, inner, Context) { }

    /// <summary>
    /// Converts the exception to a string for displaying
    /// </summary>
    /// <returns>The error message</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Message);
        if (Image is not null)
        {
            sb.AppendLine("Image:");
            sb.AppendLine($"\tWorking Directory: {Image.WorkingDirectory}");
            sb.AppendLine($"\tFile Name: {Image.FileName}");
        }
        if (Context is not null)
        {
            sb.AppendLine("Context:");
            foreach (var ctx in Context)
            {
                if (ctx is null) continue;
                sb.AppendLine($"\t{ctx.ExceptionString()}");
            }
        }
        sb.AppendLine("Stack: " + StackTrace);
        return sb.ToString();
    }
}
