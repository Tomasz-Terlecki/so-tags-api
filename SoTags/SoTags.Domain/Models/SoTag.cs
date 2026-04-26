namespace SoTags.Domain.Models;

public record SoTag : ExternalDataModel
{
    public string Name { get; init; }
    public string Description { get; init; }

    public SoTag(Guid Id, string Name, string Description) : base(Id)
    {
        this.Name = Name;
        this.Description = Description;
    }
}
