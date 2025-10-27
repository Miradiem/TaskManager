using Api.Features.Tasks.Common;
using Api.Features.Tasks.Update;
using FluentAssertions;

namespace TaskManager.Tests.Validators;

public class UpdateTaskRequestValidatorTests
{
    private readonly UpdateTaskRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldSucceed_WhenValid()
    {
        var request = new UpdateTaskRequest(
            Title: "Updated Task",
            Description: "Updated Description",
            Status: Api.Features.Tasks.Common.TaskStatus.InProgress,
            DueDate: DateTime.UtcNow.AddDays(2)
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenTitleIsEmpty()
    {
        var request = new UpdateTaskRequest(
            Title: "",
            Description: "Description",
            Status: Api.Features.Tasks.Common.TaskStatus.New,
            DueDate: null
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_ShouldFail_WhenTitleExceedsMaxLength()
    {
        var request = new UpdateTaskRequest(
            Title: new string('A', 201),
            Description: "Description",
            Status: Api.Features.Tasks.Common.TaskStatus.New,
            DueDate: null
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Validate_ShouldFail_WhenDescriptionExceedsMaxLength()
    {
        var request = new UpdateTaskRequest(
            Title: "Valid Title",
            Description: new string('A', 1001),
            Status: Api.Features.Tasks.Common.TaskStatus.New,
            DueDate: null
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_ShouldFail_WhenStatusIsInvalid()
    {
        var request = new UpdateTaskRequest(
            Title: "Valid Title",
            Description: "Description",
            Status: (Api.Features.Tasks.Common.TaskStatus)999,
            DueDate: null
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Status");
    }
}