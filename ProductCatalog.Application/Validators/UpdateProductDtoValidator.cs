using FluentValidation;
using ProductCatalog.Application.DTOs;

namespace ProductCatalog.Application.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nome é obrigatório")
            .MaximumLength(200)
            .WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Descrição é obrigatória")
            .MaximumLength(1000)
            .WithMessage("Descrição deve ter no máximo 1000 caracteres");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Preço deve ser maior que zero");

        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Categoria é obrigatória")
            .MaximumLength(100)
            .WithMessage("Categoria deve ter no máximo 100 caracteres");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status deve ser um valor válido (Active ou Inactive)");
    }
}