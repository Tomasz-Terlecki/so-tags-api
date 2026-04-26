namespace SoTags.Domain.Models;

public record SoTag : ExternalDataModel
{
    public bool HasSynonyms { get; init; }
    public bool IsModeratorOnly { get; init; }
    public bool IsRequired { get; init; }
    public int Count { get; init; }
    public string Name { get; init; }

    public SoTag(Guid Id, bool HasSynonyms, bool IsModeratorOnly, bool IsRequired, int Count, string Name) : base(Id)
    {
        this.HasSynonyms = HasSynonyms;
        this.IsModeratorOnly = IsModeratorOnly;
        this.IsRequired = IsRequired;
        this.Count = Count;
        this.Name = Name;
    }
}
