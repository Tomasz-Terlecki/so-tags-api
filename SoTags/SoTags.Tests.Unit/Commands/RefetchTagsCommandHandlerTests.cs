using Xunit;
using FluentAssertions;
using Moq;
using SoTags.Domain.Commands;
using SoTags.Domain.Models;
using SoTags.Domain.Interfaces.DataProviders;
using SoTags.Domain.Interfaces.Repositories;

namespace SoTags.Tests.Unit.Commands;

public class RefetchTagsCommandHandlerTests
{
    private readonly Mock<ISoTagProvider> _mockProvider;
    private readonly Mock<ISoTagRepository> _mockRepository;
    private readonly RefetchTagsCommandHandler _handler;

    public RefetchTagsCommandHandlerTests()
    {
        _mockProvider = new Mock<ISoTagProvider>();
        _mockRepository = new Mock<ISoTagRepository>();
        _handler = new RefetchTagsCommandHandler(_mockProvider.Object, _mockRepository.Object);
    }

    private List<SoTag> CreateTestTags(int count)
    {
        var tags = new List<SoTag>();
        for (int i = 1; i <= count; i++)
        {
            tags.Add(new SoTag(
                Guid.NewGuid(),
                HasSynonyms: i % 2 == 0,
                IsModeratorOnly: false,
                IsRequired: false,
                Count: 1000 - i,
                Name: $"tag{i}"
            ));
        }
        return tags;
    }

    [Fact]
    public async Task Handle_WithValidCommand_RefetchesAndUpdatesTags()
    {
        // Arrange
        var newTags = CreateTestTags(1000);
        const int count = 1000;

        _mockRepository.Setup(r => r.RemoveAllAsync())
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(count);
        _mockProvider.Setup(p => p.GetAsync(count, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTags);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<SoTag>()))
            .Returns(Task.CompletedTask);

        var command = new RefetchTagsCommand(count);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(count);
        _mockRepository.Verify(r => r.RemoveAllAsync(), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Exactly(2)); // Once after RemoveAll, once after AddAsync
        _mockProvider.Verify(p => p.GetAsync(count, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<SoTag>()), Times.Exactly(count));
    }

    [Fact]
    public async Task Handle_WithDefaultCount_Uses1000()
    {
        // Arrange
        const int defaultCount = 1000;
        var newTags = CreateTestTags(defaultCount);

        _mockRepository.Setup(r => r.RemoveAllAsync())
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(defaultCount);
        _mockProvider.Setup(p => p.GetAsync(defaultCount, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTags);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<SoTag>()))
            .Returns(Task.CompletedTask);

        var command = new RefetchTagsCommand();

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(defaultCount);
        _mockProvider.Verify(p => p.GetAsync(defaultCount, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ClearsExistingTagsFirst()
    {
        // Arrange
        var newTags = CreateTestTags(100);
        var callOrder = new List<string>();

        _mockRepository.Setup(r => r.RemoveAllAsync())
            .Callback(() => callOrder.Add("RemoveAll"))
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .Callback(() => callOrder.Add("SaveChanges"))
            .ReturnsAsync(100);
        _mockProvider.Setup(p => p.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("GetAsync"))
            .ReturnsAsync(newTags);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<SoTag>()))
            .Returns(Task.CompletedTask);

        var command = new RefetchTagsCommand(100);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        callOrder[0].Should().Be("RemoveAll");
        callOrder[1].Should().Be("SaveChanges");
        callOrder[2].Should().Be("GetAsync");
    }

    [Fact]
    public async Task Handle_WhenSaveResultDoesNotMatchCount_ReturnsMinusOne()
    {
        // Arrange
        var newTags = CreateTestTags(100);

        _mockRepository.Setup(r => r.RemoveAllAsync())
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(50); // Mismatch: expected 100, got 50
        _mockProvider.Setup(p => p.GetAsync(100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTags);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<SoTag>()))
            .Returns(Task.CompletedTask);

        var command = new RefetchTagsCommand(100);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(-1);
    }

    [Fact]
    public async Task Handle_WhenProviderThrowsException_PropagatesException()
    {
        // Arrange
        _mockRepository.Setup(r => r.RemoveAllAsync())
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);
        _mockProvider.Setup(p => p.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("API Error"));

        var command = new RefetchTagsCommand(100);

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_AddsAllFetchedTags()
    {
        // Arrange
        var count = 50;
        var newTags = CreateTestTags(count);
        var addedTags = new List<SoTag>();

        _mockRepository.Setup(r => r.RemoveAllAsync())
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(count);
        _mockProvider.Setup(p => p.GetAsync(count, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTags);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<SoTag>()))
            .Callback<SoTag>(t => addedTags.Add(t))
            .Returns(Task.CompletedTask);

        var command = new RefetchTagsCommand(count);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        addedTags.Should().HaveCount(count);
        foreach (var newTag in newTags)
        {
            addedTags.Should().Contain(t => t.Name == newTag.Name && t.Count == newTag.Count);
        }
    }

    [Fact]
    public async Task Handle_WithEmptyProviderResult_ReturnsZero()
    {
        // Arrange
        _mockRepository.Setup(r => r.RemoveAllAsync())
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(0);
        _mockProvider.Setup(p => p.GetAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SoTag>());

        var command = new RefetchTagsCommand(100);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(0);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(2000)]
    public async Task Handle_WithVariousCountValues_WorksCorrectly(int count)
    {
        // Arrange
        var newTags = CreateTestTags(count);

        _mockRepository.Setup(r => r.RemoveAllAsync())
            .Returns(Task.CompletedTask);
        _mockRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(count);
        _mockProvider.Setup(p => p.GetAsync(count, It.IsAny<CancellationToken>()))
            .ReturnsAsync(newTags);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<SoTag>()))
            .Returns(Task.CompletedTask);

        var command = new RefetchTagsCommand(count);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(count);
    }
}
