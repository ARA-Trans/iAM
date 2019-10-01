﻿using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;

namespace BridgeCare.ExceptionHandling
{
    /// <summary>
    /// Represents the an attribute that provides a filter for unhandled exceptions.
    /// </summary>
    public class UnhandledExceptionFilterAttribute : ExceptionFilterAttribute
    {
        #region UnhandledExceptionFilterAttribute()

        #endregion

        #region DefaultHandler
        /// <summary>
        /// Gets a delegate method that returns an <see cref="HttpResponseMessage"/> 
        /// that describes the supplied exception.
        /// </summary>
        /// <value>
        /// A <see cref="Func{T1,T2,TResult}"/> delegate method that returns 
        /// an <see cref="HttpResponseMessage"/> that describes the supplied exception.
        /// </value>
        private static Func<Exception, HttpRequestMessage, HttpResponseMessage> DefaultHandler = (exception, request) =>
        {
            if (exception == null)
            {
                return null;
            }

            var response = request.CreateResponse(
                HttpStatusCode.InternalServerError, GetContentOf(exception)
            );
            response.ReasonPhrase = exception.Message.Replace(Environment.NewLine, String.Empty);

            return response;
        };
        #endregion

        #region GetContentOf
        /// <summary>
        /// Gets a delegate method that extracts information from the specified exception.
        /// </summary>
        /// <value>
        /// A <see cref="Func{Exception, String}"/> delegate method that extracts information 
        /// from the specified exception.
        /// </value>
        private static Func<Exception, string> GetContentOf = exception =>
        {
            if (exception == null)
            {
                return String.Empty;
            }

            var result = new StringBuilder();

            result.AppendLine(exception.Message);
            result.AppendLine();

            Exception innerException = exception.InnerException;
            while (innerException != null)
            {
                result.AppendLine(innerException.Message);
                result.AppendLine();
                innerException = innerException.InnerException;
            }

#if DEBUG
            result.AppendLine(exception.StackTrace);
#endif

            return result.ToString();
        };
        #endregion

        #region Handlers
        /// <summary>
        /// Gets the exception handlers registered with this filter.
        /// </summary>
        /// <value>
        /// A <see cref="ConcurrentDictionary{TKey,TValue}"/> collection that contains 
        /// the exception handlers registered with this filter.
        /// </value>
        protected ConcurrentDictionary<Type, Tuple<HttpStatusCode?, Func<Exception, HttpRequestMessage, HttpResponseMessage>>> Handlers
        {
            get
            {
                return _filterHandlers;
            }
        }
        private readonly ConcurrentDictionary<Type, Tuple<HttpStatusCode?, Func<Exception, HttpRequestMessage, HttpResponseMessage>>> _filterHandlers = new ConcurrentDictionary<Type, Tuple<HttpStatusCode?, Func<Exception, HttpRequestMessage, HttpResponseMessage>>>();
        #endregion

        #region OnException(HttpActionExecutedContext actionExecutedContext)
        /// <summary>
        /// Raises the exception event.
        /// </summary>
        /// <param name="actionExecutedContext">The context for the action.</param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext == null || actionExecutedContext.Exception == null)
            {
                return;
            }

            var type = actionExecutedContext.Exception.GetType();

            Tuple<HttpStatusCode?, Func<Exception, HttpRequestMessage, HttpResponseMessage>> registration = null;

            if (Handlers.TryGetValue(type, out registration))
            {
                var statusCode = registration.Item1;
                var handler = registration.Item2;

                var response = handler(
                    actionExecutedContext.Exception.GetBaseException(),
                    actionExecutedContext.Request
                );

                // Use registered status code if available
                if (statusCode.HasValue)
                {
                    response.StatusCode = statusCode.Value;
                }

                actionExecutedContext.Response = response;
            }
            else
            {
                // If no exception handler registered for the exception type, fallback to default handler
                actionExecutedContext.Response = DefaultHandler(
                    actionExecutedContext.Exception.GetBaseException(), actionExecutedContext.Request
                );
            }
        }
        #endregion

        #region Register<TException>(HttpStatusCode statusCode)
        /// <summary>
        /// Registers an exception handler that returns the specified status code for exceptions of type <typeparamref name="TException"/>.
        /// </summary>
        /// <typeparam name="TException">The type of exception to register a handler for.</typeparam>
        /// <param name="statusCode">The HTTP status code to return for exceptions of type <typeparamref name="TException"/>.</param>
        /// <returns>
        /// This <see cref="UnhandledExceptionFilterAttribute"/> after the exception handler has been added.
        /// </returns>
        public UnhandledExceptionFilterAttribute Register<TException>(HttpStatusCode statusCode)
            where TException : Exception
        {

            var type = typeof(TException);
            var item = new Tuple<HttpStatusCode?, Func<Exception, HttpRequestMessage, HttpResponseMessage>>(
                statusCode, DefaultHandler
            );

            if (!Handlers.TryAdd(type, item))
            {
                Tuple<HttpStatusCode?, Func<Exception, HttpRequestMessage, HttpResponseMessage>> oldItem = null;

                if (Handlers.TryRemove(type, out oldItem))
                {
                    Handlers.TryAdd(type, item);
                }
            }

            return this;
        }
        #endregion

        #region Register<TException>(Func<Exception, HttpRequestMessage, HttpResponseMessage> handler)
        /// <summary>
        /// Registers the specified exception <paramref name="handler"/> for exceptions of type <typeparamref name="TException"/>.
        /// </summary>
        /// <typeparam name="TException">The type of exception to register the <paramref name="handler"/> for.</typeparam>
        /// <param name="handler">The exception handler responsible for exceptions of type <typeparamref name="TException"/>.</param>
        /// <returns>
        /// This <see cref="UnhandledExceptionFilterAttribute"/> after the exception <paramref name="handler"/> 
        /// has been added.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <see langword="null"/>.</exception>
        public UnhandledExceptionFilterAttribute Register<TException>(Func<Exception, HttpRequestMessage, HttpResponseMessage> handler)
            where TException : Exception
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            var type = typeof(TException);
            var item = new Tuple<HttpStatusCode?, Func<Exception, HttpRequestMessage, HttpResponseMessage>>(
                null, handler
            );

            if (!Handlers.TryAdd(type, item))
            {
                Tuple<HttpStatusCode?, Func<Exception, HttpRequestMessage, HttpResponseMessage>> oldItem = null;

                if (Handlers.TryRemove(type, out oldItem))
                {
                    Handlers.TryAdd(type, item);
                }
            }

            return this;
        }
        #endregion

        #region Unregister<TException>()
        /// <summary>
        /// Unregisters the exception handler for exceptions of type <typeparamref name="TException"/>.
        /// </summary>
        /// <typeparam name="TException">The type of exception to unregister handlers for.</typeparam>
        /// <returns>
        /// This <see cref="UnhandledExceptionFilterAttribute"/> after the exception handler 
        /// for exceptions of type <typeparamref name="TException"/> has been removed.
        /// </returns>
        public UnhandledExceptionFilterAttribute Unregister<TException>()
            where TException : Exception
        {
            Tuple<HttpStatusCode?, Func<Exception, HttpRequestMessage, HttpResponseMessage>> item = null;

            Handlers.TryRemove(typeof(TException), out item);

            return this;
        }
        #endregion
    }
}