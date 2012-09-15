using System;

namespace Embedly.OEmbed
{
    /// <summary>
    /// Represents an oEmbed result
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="response">The response.</param>
        public Result(UrlRequest request, Response response)
        {
            Request = request;
            Response = response;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="exception">The exception.</param>
        public Result(UrlRequest request, Exception exception)
        {
            Request = request;
            Exception = exception;
        }

        /// <summary>
        /// Gets the original request information.
        /// </summary>
        public UrlRequest Request { get; private set; }

        /// <summary>
        /// Gets the response.
        /// </summary>
        public Response Response { get; private set; }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Exception { get; private set; }
    }
}