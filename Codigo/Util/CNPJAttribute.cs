using System.ComponentModel.DataAnnotations;

namespace Util
{
    /// <summary>
    /// Validação customizada para CNPJ
    /// </summary>
    public class CNPJAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;

            var valueNoEspecial = Methods.RemoveNaoNumericos((string)value);
            bool valido = Methods.ValidarCnpj(valueNoEspecial);

            return valido;
        }

        public override string FormatErrorMessage(string name)
        {
            return "CNPJ Inválido";
        }
    }
}