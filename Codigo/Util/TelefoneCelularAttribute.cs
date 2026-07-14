using System.ComponentModel.DataAnnotations;


namespace Util;

/// <summary>
/// Validação customizada para CPF
/// </summary>
public class TelefoneCelularAttribute : ValidationAttribute
{
    /// <summary>
    /// Construtor
    /// </summary>
    public TelefoneCelularAttribute() { }

    /// <summary>
    /// Validação server
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return true;
        // Remove tudo que não for número antes de validar (não permitir letras)
        var onlyDigits = Methods.RemoveNaoNumericos((string)value);
        if (onlyDigits.Length != 11)
            return false;
        if (onlyDigits.StartsWith("0"))
            return false;
        return true;
    }

    public string GetErrorMessage() =>
        $"Celular Inválido";
}
