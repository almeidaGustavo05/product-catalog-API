using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ProductCatalog.Application.Validators;

public class ImageUploadValidator : AbstractValidator<IFormFile>
{
    private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif" };
    private const int MaxFileSizeInBytes = 5 * 1024 * 1024;

    public ImageUploadValidator()
    {
        RuleFor(x => x)
            .NotNull()
            .WithMessage("Nenhuma imagem foi enviada");

        RuleFor(x => x.Length)
            .GreaterThan(0)
            .WithMessage("Arquivo de imagem está vazio")
            .LessThanOrEqualTo(MaxFileSizeInBytes)
            .WithMessage("Arquivo muito grande. Tamanho máximo: 5MB");

        RuleFor(x => x.ContentType)
            .Must(contentType => AllowedContentTypes.Contains(contentType?.ToLower()))
            .WithMessage("Tipo de arquivo não suportado. Use JPEG, PNG ou GIF");
    }
}