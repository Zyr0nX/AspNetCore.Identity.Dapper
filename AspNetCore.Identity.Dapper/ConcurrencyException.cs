namespace AspNetCore.Identity.Dapper;

public class ConcurrencyException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DbUpdateConcurrencyException" /> class.
    /// </summary>
    public ConcurrencyException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DbUpdateConcurrencyException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ConcurrencyException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DbUpdateConcurrencyException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ConcurrencyException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}