using System;

namespace WhatsPro.Exceptions;

/// <summary>
/// Exception thrown when the API returns a validation error (e.g. HTTP 422).
/// </summary>
public class ValidationException : WhatsProException
{
    public ValidationException(string message) : base(message) { }
}
