using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkoutOptimization.Logic.Helpers
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if(!context.ModelState.IsValid)
            {
                var error= new ErrorModel(
                    String.Join(',',context.ModelState.Values.SelectMany(x => x.Errors.Select(x => x.ErrorMessage)).ToArray())
                    );
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Result = new JsonResult(error);
            }
      
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
           
        }
    }
}
