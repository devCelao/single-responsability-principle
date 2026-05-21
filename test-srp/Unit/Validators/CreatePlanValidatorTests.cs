using application.Dtos;
using application.Validators;
using FluentAssertions;
using test_srp.Helpers;

namespace test_srp.Unit.Validators;

public class CreatePlanValidatorTests
{
    private readonly CreatePlanValidator _sut;

    public CreatePlanValidatorTests()
    {
        _sut = new CreatePlanValidator();
    }

    [Fact]
    public void Validate_ValidPlan_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto();

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyName_ShouldHaveError()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto(name: "");

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required.");
    }

    [Fact]
    public void Validate_NullName_ShouldHaveError()
    {
        // Arrange
        var dto = new CreatePlanDto
        {
            Name = null!,
            Description = "Valid description",
            Price = 100m
        };

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameTooLong_ShouldHaveError()
    {
        // Arrange
        var longName = new string('A', 101);
        var dto = TestDataBuilder.CreateValidPlanDto(name: longName);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name cannot exceed 100 characters.");
    }

    [Fact]
    public void Validate_NameExactly100Chars_ShouldNotHaveError()
    {
        // Arrange
        var exactName = new string('A', 100);
        var dto = TestDataBuilder.CreateValidPlanDto(name: exactName);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("100 characters"));
    }

    [Fact]
    public void Validate_PriceZero_ShouldHaveError()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto(price: 0);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price" && e.ErrorMessage == "Price must be greater than zero.");
    }

    [Fact]
    public void Validate_PriceNegative_ShouldHaveError()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto(price: -10m);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price" && e.ErrorMessage == "Price must be greater than zero.");
    }

    [Fact]
    public void Validate_PricePositive_ShouldNotHaveError()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidPlanDto(price: 0.01m);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_DescriptionTooLong_ShouldHaveError()
    {
        // Arrange
        var longDescription = new string('A', 501);
        var dto = TestDataBuilder.CreateValidPlanDto(description: longDescription);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage == "Description cannot exceed 500 characters.");
    }

    [Fact]
    public void Validate_DescriptionExactly500Chars_ShouldNotHaveError()
    {
        // Arrange
        var exactDescription = new string('A', 500);
        var dto = TestDataBuilder.CreateValidPlanDto(description: exactDescription);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "Description" && e.ErrorMessage.Contains("500 characters"));
    }

    [Fact]
    public void Validate_MultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var dto = TestDataBuilder.CreateInvalidPlanDto();

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(1);
    }
}
