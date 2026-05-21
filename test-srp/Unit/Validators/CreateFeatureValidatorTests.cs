using application.Dtos;
using application.Validators;
using FluentAssertions;
using test_srp.Helpers;

namespace test_srp.Unit.Validators;

public class CreateFeatureValidatorTests
{
    private readonly CreateFeatureValidator _sut;

    public CreateFeatureValidatorTests()
    {
        _sut = new CreateFeatureValidator();
    }

    [Fact]
    public void Validate_ValidFeature_ShouldNotHaveErrors()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidFeatureDto();

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
        var dto = TestDataBuilder.CreateValidFeatureDto(name: "");

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
        var dto = new CreateFeatureDto
        {
            Name = null!,
            Permission = "valid:permission"
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
        var dto = TestDataBuilder.CreateValidFeatureDto(name: longName);

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
        var dto = TestDataBuilder.CreateValidFeatureDto(name: exactName);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("100 characters"));
    }

    [Fact]
    public void Validate_EmptyPermission_ShouldHaveError()
    {
        // Arrange
        var dto = TestDataBuilder.CreateValidFeatureDto(permission: "");

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Permission" && e.ErrorMessage == "Permission is required.");
    }

    [Fact]
    public void Validate_NullPermission_ShouldHaveError()
    {
        // Arrange
        var dto = new CreateFeatureDto
        {
            Name = "Valid Name",
            Permission = null!
        };

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Permission");
    }

    [Fact]
    public void Validate_PermissionTooLong_ShouldHaveError()
    {
        // Arrange
        var longPermission = new string('A', 501);
        var dto = TestDataBuilder.CreateValidFeatureDto(permission: longPermission);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Permission" && e.ErrorMessage == "Permission cannot exceed 500 characters.");
    }

    [Fact]
    public void Validate_PermissionExactly500Chars_ShouldNotHaveError()
    {
        // Arrange
        var exactPermission = new string('A', 500);
        var dto = TestDataBuilder.CreateValidFeatureDto(permission: exactPermission);

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.Errors.Should().NotContain(e => e.PropertyName == "Permission" && e.ErrorMessage.Contains("500 characters"));
    }

    [Fact]
    public void Validate_MultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var dto = TestDataBuilder.CreateInvalidFeatureDto();

        // Act
        var result = _sut.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
