﻿using System;
using System.Text.Json.Serialization;

namespace Bluesky.NET.Models;

public class FeedRecord
{
    [JsonPropertyName("$type")]
    public string Type { get; init; } = string.Empty;

    public string? Uri { get; init; }

    public string? Cid { get; init; }

    public DateTime CreatedAt { get; init; }

    public string? Description { get; init; }

    public string? Name { get; init; }

    public DateTime UpdatedAt { get; init; }

    public string Text { get; init; } = string.Empty;

    public ReplyRecord? Reply { get; init; }

    public PostEmbed[]? Embeds { get; init; }

    public Author? Author { get; init; }

    public FeedRecord? Value { get; init; } // fyi for future, I think this could be anything, not necessarily a record

    public FeedRecord? Record { get; init; }

    public Author? Creator { get; init; }
}
