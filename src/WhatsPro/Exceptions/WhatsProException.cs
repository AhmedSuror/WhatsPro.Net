using System;

namespace WhatsPro.Exceptions;

/// <summary>
/// Base exception for all WhatsPro.Net SDK exceptions.
/// </summary>
public class WhatsProException : Exception
{
    public WhatsProException(string message) : base(message) { }
    public WhatsProException(string message, Exception innerException) : base(message, innerException) { }
}
