using Api.Features.Tasks.Common;
using Api.Features.Tasks.Create;
using FluentAssertions;

namespace TaskManager.Tests.Validators;

public class CreateTaskRequestValidatorTests
{
    private readonly CreateTaskRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldSucceed_WhenValid()
    {
        var request = new CreateTaskRequest(
            Title: "Test Task",
            Description: "Test Description",
            DueDate: DateTime.UtcNow.AddDays(1)
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenTitleIsEmpty()
    {
        var request = new CreateTaskRequest(
            Title: "",
            Description: "Test Description"
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_ShouldFail_WhenTitleExceedsMaxLength()
    {
        var request = new CreateTaskRequest(
            Title: new string('A', 201),
            Description: "Test Description"
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title" && e.ErrorMessage.Contains("200"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenDescriptionExceedsMaxLength()
    {
        var request = new CreateTaskRequest(
            Title: "Test Task",
            Description: new string('A', 1001)
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage.Contains("1000"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenDueDateIsInThePast()
    {
        var request = new CreateTaskRequest(
            Title: "Test Task",
            Description: "Test Description",
            DueDate: DateTime.UtcNow.AddDays(-1)
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DueDate" && e.ErrorMessage.Contains("future"));
    }

    [Fact]
    public void Validate_ShouldSucceed_WhenDueDateIsNull()
    {
        var request = new CreateTaskRequest(
            Title: "Test Task",
            Description: "Test Description",
            DueDate: null
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }
}