using application.Dtos;
using application.Validators;
using FluentAssertions;
using test_srp.Helpers;

namespace test_srp.Unit.Validators;

public class CreateServiceValidatorTests
{
    private readonly CreateServiceValidator _sut;

    public CreateServiceValidatorTests()
    {
        _sut = new CreateServiceValidator();
    }

    [Fact]
    public void Validate_ValidService_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidServiceDto();

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
        var dto = TestDataBuilder.CreateValidServiceDto(name: "");

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
        var dto = new CreateServiceDto
        {
            Name = null!,
            Type = "Valid Type"
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
        var longName = new string('A', 501);
        var dto = TestDataBuilder.CreateValidServiceDto(name: longName);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name cannot exceed 500 characters.");
    }

    [Fact]
    public void Validate_EmptyType_ShouldHaveError()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidServiceDto(type: "");

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type" && e.ErrorMessage == "Type is required.");
    }

    [Fact]
    public void Validate_NullType_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateServiceDto
        {
            Name = "Valid Name",
            Type = null!
        };

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }

    [Fact]
    public void Validate_TypeTooLong_ShouldHaveError()
    {
        // Arrange
        var longType = new string('A', 101);
        var dto = TestDataBuilder.CreateValidServiceDto(type: longType);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type" && e.ErrorMessage == "Type cannot exceed 100 characters.");
    }

    [Fact]
    public void Validate_MultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var dto = TestDataBuilder.CreateInvalidServiceDto();

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
