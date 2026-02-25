using DeliFitAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DeliFitAPI.Filter
{
    public class CartaoValidationFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Verifica se existe um CartaoViewModel nos argumentos da action
            var cartaoViewModel = context.ActionArguments.Values
                .OfType<CartaoViewModel>()
                .FirstOrDefault();

            if (cartaoViewModel == null)
                return;

            // Valida o ModelState
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
                return;
            }

            // Valida se a data de validade não está no passado
            if (cartaoViewModel.Validade < DateTime.Now)
            {
                context.ModelState.AddModelError("Validade", "O cartão está vencido.");
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}