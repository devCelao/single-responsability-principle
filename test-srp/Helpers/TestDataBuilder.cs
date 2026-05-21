using application.Dtos;
using domain.Entities;

namespace test_srp.Helpers;

public static class TestDataBuilder
{
    public static CreatePlanDto CreateValidPlanDto(
        string name = "Plano Teste",
        string description = "Descricao do plano teste",
        decimal price = 99.90m,
        bool isActive = true) => new()
    {
        Name = name,
        Description = description,
        Price = price,
        IsActive = isActive
    };

    public static CreatePlanDto CreateInvalidPlanDto() => new()
    {
        Name = "",
        Description = "",
        Price = 0,
        IsActive = false
    };

    public static Plan CreatePlan(
        string name = "Plano",
        string description = "Descricao",
        decimal price = 100m)
    {
        return new Plan(name, description, price);
    }

    public static CreateServiceDto CreateValidServiceDto(
        string name = "Servico Teste",
        string type = "Premium") => new()
    {
        Name = name,
        Type = type
    };

    public static CreateServiceDto CreateInvalidServiceDto() => new()
    {
        Name = "",
        Type = ""
    };

    public static Service CreateService(
        string name = "Servico",
        string type = "Standard")
    {
        return new Service(name, type);
    }

    public static CreateFeatureDto CreateValidFeatureDto(
        string name = "Feature Teste",
        string permission = "read:all") => new()
    {
        Name = name,
        Permission = permission
    };

    public static CreateFeatureDto CreateInvalidFeatureDto() => new()
    {
        Name = "",
        Permission = ""
    };

    public static Feature CreateFeature(
        string name = "Feature",
        string permission = "read")
    {
        return new Feature(name, permission);
    }
}
