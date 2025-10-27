using FluentValidation;

namespace Api.Features.Tasks.Update;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid value");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.MinValue).WithMessage("Due date must be a valid date")
            .When(x => x.DueDate.HasValue);
    }
}