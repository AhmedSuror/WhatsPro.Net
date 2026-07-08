using System;

namespace WhatsPro.Exceptions;

/// <summary>
/// Exception thrown when the Whats-Pro API returns an unsuccessful response.
/// </summary>
public class ApiException : WhatsProException
{
    public ApiException(string message) : base(message) { }
}
