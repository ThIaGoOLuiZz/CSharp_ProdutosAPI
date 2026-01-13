using Microsoft.AspNetCore.Mvc.Filters;

namespace ProdutosAPI.Filters
{
    public class ApiLoggingFilter : IActionFilter
    {
        private readonly ILogger<ApiLoggingFilter> _logger;

        public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //Executa depois do metodo action
            _logger.LogInformation("### Executando -> OnActionExecuted");
            _logger.LogInformation("##################################################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"ModelState : {context.ModelState.IsValid}");
            _logger.LogInformation("##################################################");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //Executa antes do metodo action
            _logger.LogInformation("### Executando -> OnActionExecuting");
            _logger.LogInformation("##################################################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"ModelState : {context.HttpContext.Response.StatusCode}");
            _logger.LogInformation("##################################################");
        }
    }
}
