using System.ComponentModel.DataAnnotations;

namespace Util
{
    public class CepAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return true;

            var valueNoEspecial = Methods.RemoveNaoNumericos(value.ToString());

            // Só confere se tem 8 números. Tiramos a verificação do zero no começo!
            if (valueNoEspecial.Length != 8)
                return false;

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"O campo {name} deve conter um CEP válido de 8 dígitos.";
        }
    }
}