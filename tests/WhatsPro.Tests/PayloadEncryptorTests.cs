using System;
using WhatsPro.Security;

namespace WhatsPro.Tests;

public class PayloadEncryptorTests
{
    private const string DefaultKey = "abcd123456789ABCD";

    [Fact]
    public void Encrypt_Decrypt_RoundTrip_Works()
    {
        // Arrange
        string original = "{\"test\":123}";
        
        // Act
        string encrypted = PayloadEncryptor.Encrypt(original, DefaultKey);
        string decrypted = PayloadEncryptor.Decrypt(encrypted, DefaultKey);
        
        // Assert
        Assert.NotEqual(original, encrypted);
        Assert.Equal(original, decrypted);
        Assert.True(encrypted.StartsWith("U2FsdGVkX1"), "Should start with Base64 'Salted__'");
    }
}
