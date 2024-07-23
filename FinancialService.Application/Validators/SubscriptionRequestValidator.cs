using FinancialService.Application.Models.Request;
using FluentValidation;

namespace FinancialService.Application.Validators;

public class SubscriptionRequestValidator : AbstractValidator<SubscriptionRequest>
{
    public SubscriptionRequestValidator()
    {
        RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage("Symbol is required.");
    }
}