using System.Collections.Generic;

namespace WhatsPro.Models;

/// <summary>
/// A shared request model for deleting items by their IDs.
/// </summary>
public class DeleteRequest
{
    public List<int> Ids { get; set; } = new List<int>();
}
