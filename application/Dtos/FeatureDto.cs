namespace application.Dtos;

public class CreateFeatureDto
{
    public string Name { get; set; } = default!;
    public string Permission { get; set; } = default!;
}

public class FeatureDto : CreateFeatureDto
{
    public Guid Id { get; set; }
}