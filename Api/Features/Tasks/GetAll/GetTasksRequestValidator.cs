using FluentValidation;

namespace Api.Features.Tasks.GetAll;

public class GetTasksRequestValidator : AbstractValidator<GetTasksRequest>
{
    public GetTasksRequestValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid value")
            .When(x => x.Status.HasValue);

        RuleFor(x => x.DueDateFrom)
            .Must(date => !date.HasValue || date.Value.Year >= 1900 && date.Value.Year <= 2100)
            .WithMessage("DueDateFrom must be a valid date between 1900 and 2100")
            .When(x => x.DueDateFrom.HasValue);

        RuleFor(x => x.DueDateTo)
            .Must(date => !date.HasValue || date.Value.Year >= 1900 && date.Value.Year <= 2100)
            .WithMessage("DueDateTo must be a valid date between 1900 and 2100")
            .When(x => x.DueDateTo.HasValue);

        RuleFor(x => x.DueDateFrom)
            .LessThanOrEqualTo(x => x.DueDateTo).WithMessage("DueDateFrom must be less than or equal to DueDateTo")
            .When(x => x.DueDateFrom.HasValue && x.DueDateTo.HasValue);

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100");
    }
}