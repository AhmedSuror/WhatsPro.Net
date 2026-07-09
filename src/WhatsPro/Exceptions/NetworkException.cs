using System;

namespace WhatsPro.Exceptions;

/// <summary>
/// Exception thrown when a network or transport-level error occurs during the API request.
/// </summary>
public class NetworkException : WhatsProException
{
    public NetworkException(string message) : base(message) { }
    public NetworkException(string message, Exception innerException) : base(message, innerException) { }
}
