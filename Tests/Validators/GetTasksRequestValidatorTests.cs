using Api.Features.Tasks.Common;
using Api.Features.Tasks.GetAll;
using FluentAssertions;

namespace TaskManager.Tests.Validators;

public class GetTasksRequestValidatorTests
{
    private readonly GetTasksRequestValidator _validator = new();

    [Fact]
    public void Validate_ShouldSucceed_WhenValid()
    {
        var request = new GetTasksRequest(
            Status: Api.Features.Tasks.Common.TaskStatus.New,
            DueDateFrom: DateTime.UtcNow.AddDays(-7),
            DueDateTo: DateTime.UtcNow.AddDays(7),
            Page: 1,
            PageSize: 10
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ShouldFail_WhenPageIsZero()
    {
        var request = new GetTasksRequest(Page: 0);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Page");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPageIsNegative()
    {
        var request = new GetTasksRequest(Page: -1);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Page");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPageSizeIsZero()
    {
        var request = new GetTasksRequest(PageSize: 0);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPageSizeExceeds100()
    {
        var request = new GetTasksRequest(PageSize: 101);

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PageSize" && e.ErrorMessage.Contains("100"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenDueDateFromIsGreaterThanDueDateTo()
    {
        var request = new GetTasksRequest(
            DueDateFrom: DateTime.UtcNow.AddDays(7),
            DueDateTo: DateTime.UtcNow.AddDays(1)
        );

        var result = _validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DueDateFrom");
    }
}