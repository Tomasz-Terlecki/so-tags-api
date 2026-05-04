using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Xunit;

namespace SoTags.Tests.Integration;

public class SoTagsController_Get_IntegrationTests
{
    private static string ApiBaseUrl =>
        Environment.GetEnvironmentVariable("API_BASE_URL")
        ?? throw new InvalidOperationException("Missing env var API_BASE_URL (e.g. http://api:8080).");

    private static string ConnectionString =>
        Environment.GetEnvironmentVariable("TEST_CONNECTION_STRING")
        ?? throw new InvalidOperationException("Missing env var TEST_CONNECTION_STRING.");

    [Fact]
    public async Task Get_returns_paginated_response_and_includes_seeded_row()
    {
        // Arrange: insert one row into the real MSSQL used by the running API.
        var tagId = Guid.NewGuid();
        var tagName = $"it-tag-{tagId}";

        await using (var conn = new SqlConnection(ConnectionString))
        {
            await conn.OpenAsync();

            // Same DB is reused across runs; without this, totalCount reflects old rows too.
            await using (var clear = new SqlCommand("DELETE FROM dbo.SoTags;", conn))
            {
                await clear.ExecuteNonQueryAsync();
            }

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
INSERT INTO dbo.SoTags (Id, HasSynonyms, IsModeratorOnly, IsRequired, [Count], [Name], [Share])
VALUES (@Id, 0, 0, 0, 123, @Name, 0.0000);";
            cmd.Parameters.AddWithValue("@Id", tagId);
            cmd.Parameters.AddWithValue("@Name", tagName);

            var affected = await cmd.ExecuteNonQueryAsync();
            affected.Should().Be(1);
        }

        using var http = new HttpClient { BaseAddress = new Uri(ApiBaseUrl) };

        // Act
        var res = await http.GetAsync($"/SoTags?pageNumber=1&pageSize=50");

        // Assert: HTTP
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert: payload shape + our row is present
        var json = await res.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        doc.RootElement.TryGetProperty("items", out var items).Should().BeTrue();
        items.ValueKind.Should().Be(JsonValueKind.Array);

        doc.RootElement.TryGetProperty("pageNumber", out var pageNumber).Should().BeTrue();
        pageNumber.GetInt32().Should().Be(1);

        doc.RootElement.TryGetProperty("pageSize", out var pageSize).Should().BeTrue();
        pageSize.GetInt32().Should().Be(50);

        doc.RootElement.TryGetProperty("totalCount", out var totalCount).Should().BeTrue();
        totalCount.GetInt32().Should().Be(1);

        var hasSeeded = items.EnumerateArray().Any(e =>
            e.TryGetProperty("name", out var n) && n.GetString() == tagName);

        hasSeeded.Should().BeTrue($"response should include the seeded tag '{tagName}'");
    }
}

