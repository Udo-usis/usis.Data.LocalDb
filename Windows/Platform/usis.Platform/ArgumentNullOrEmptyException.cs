﻿//
//  @(#) ArgumentNullOrEmptyException.cs
//
//  Project:    usis Platform
//  System:     Microsoft Visual Studio 2017
//  Author:     Udo Schäfer
//
//  Copyright (c) 2010-2017 usis GmbH. All rights reserved.

using System;
#if !WINDOWS_UWP
using System.Runtime.Serialization;
#endif

namespace usis.Platform
{
    //  ----------------------------------
    //  ArgumentNullOrEmptyException class
    //  ----------------------------------

    /// <summary>
    /// The exception that is thrown when the value of an argument
    /// is a null reference (<c>Nothing</c> in Visual Basic) or empty.
    /// </summary>

#if !WINDOWS_UWP
    [Serializable]
#endif
    public class ArgumentNullOrEmptyException : ArgumentException
    {
        #region construction

        //  ------------
        //  construction
        //  ------------

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ArgumentNullOrEmptyException" /> class.
        /// </summary>

        public ArgumentNullOrEmptyException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullOrEmptyException" /> class
        /// with the name of the parameter that causes this exception.
        /// </summary>
        /// <param name="parameterName">The name of the parameter that caused the current exception.</param>

        public ArgumentNullOrEmptyException(string parameterName) : base(Resources.Strings.ArgumentCannotBeNullOrEmpty, parameterName) { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="ArgumentNullOrEmptyException" /> class
        /// with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception,
        /// or a null reference (<c>Nothing</c> in Visual Basic)
        /// if no inner exception is specified.</param>

        public ArgumentNullOrEmptyException(string message, Exception inner) : base(message, inner) { }

#if !WINDOWS_UWP

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentNullOrWhiteSpaceException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"></see>
        /// that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"></see>
        /// that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"></see> is zero (0).
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// The info parameter is null. 
        /// </exception>

        protected ArgumentNullOrEmptyException(SerializationInfo info, StreamingContext context) : base(info, context) { }

#endif // !WINDOWS_UWP

        #endregion construction
    }
}

// eof "ArgumentNullOrEmptyException.cs"