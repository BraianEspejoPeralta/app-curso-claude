using System.ComponentModel.DataAnnotations;

namespace app_curso_claude.Models
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class MaximoDecimalesAttribute : ValidationAttribute
    {
        public int Decimales { get; }

        public MaximoDecimalesAttribute(int decimales)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(decimales);
            Decimales = decimales;
        }

        public override bool IsValid(object? value)
        {
            if (value is null)
            {
                return true;
            }

            return value is decimal valor && decimal.Round(valor, Decimales) == valor;
        }
    }
}
