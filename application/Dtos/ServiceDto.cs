namespace application.Dtos;

public class CreateServiceDto
{
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!;
}

public class ServiceDto : CreateServiceDto
{
    public Guid Id { get; set; }
   
}
