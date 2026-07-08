using System;

namespace WhatsPro.Exceptions;

/// <summary>
/// Exception thrown when authentication with the Whats-Pro API fails.
/// </summary>
public class AuthenticationException : WhatsProException
{
    public AuthenticationException(string message) : base(message) { }
}
