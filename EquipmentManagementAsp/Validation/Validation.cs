using EquipmentManagementAsp.Models;
using FluentValidation;
using System.Collections.Generic;

namespace EquipmentManagementAsp.Validation
{
    public class EquipmentValidator : AbstractValidator<Equipment>
    {
        private static readonly List<string> ValidOperators = new() { "Claro", "Tim", "Vivo" };

        public EquipmentValidator()
        {
            RuleFor(e => e.Installation)
                .NotEmpty().WithMessage("O campo 'Instalacao' é obrigatório.")
                .MaximumLength(10).WithMessage("O campo 'Instalacao' deve ter no máximo 10 caracteres.");

            RuleFor(e => e.Batch)
                .NotNull().WithMessage("O campo 'Lote' é obrigatório.")
                .InclusiveBetween(1, 10).WithMessage("O campo 'Lote' deve estar entre 1 e 10.");

            RuleFor(e => e.Operator)
                .Must(op => ValidOperators.Contains(op)).WithMessage("O campo 'Operadora' deve ser 'Claro', 'Tim' ou 'Vivo'.");

            RuleFor(e => e.Manufacturer)
                .NotEmpty().WithMessage("O campo 'Fabricante' é obrigatório.")
                .MaximumLength(15).WithMessage("O campo 'Fabricante' deve ter no máximo 15 caracteres.");

            RuleFor(e => e.Model)
                .NotNull().WithMessage("O campo 'Modelo' é obrigatório.")
                .Must(m => int.TryParse(m.ToString(), out _)).WithMessage("O campo 'Modelo' deve ser um número inteiro.");

            RuleFor(e => e.Version)
                .NotNull().WithMessage("O campo 'Versao' é obrigatório.")
                .Must(v => int.TryParse(v.ToString(), out _)).WithMessage("O campo 'Versao' deve ser um número inteiro.");
        }
    }
}
