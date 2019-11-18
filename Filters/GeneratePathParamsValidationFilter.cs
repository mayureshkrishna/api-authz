using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;


namespace apiauthz.Filters
{
    /// <summary>
    /// Path Parameter Validation Rules Filter
    /// </summary>
    public class GeneratePathParamsValidationFilter : IOperationFilter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="context">OperationFilterContext</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var pars = context.ApiDescription.ParameterDescriptions;

            foreach (var par in pars)
            {
                var swaggerParam = operation.Parameters.SingleOrDefault(p => p.Name == par.Name);

                if (par.ParameterDescriptor != null && par.ParameterDescriptor is ControllerParameterDescriptor && ((ControllerParameterDescriptor)par.ParameterDescriptor).ParameterInfo != null)
                {
                    var attributes = ((ControllerParameterDescriptor)par.ParameterDescriptor).ParameterInfo.CustomAttributes;

                    if (attributes != null && attributes.Count() > 0 && swaggerParam != null)
                    {
                        // Required - [Required]
                        var requiredAttr = attributes.FirstOrDefault(p => p.AttributeType == typeof(RequiredAttribute));
                        if (requiredAttr != null)
                        {
                            swaggerParam.Required = true;
                        }

                        // String Length [StringLength]
                        int? minLenght = null, maxLength = null;
                        var stringLengthAttr = attributes.FirstOrDefault(p => p.AttributeType == typeof(StringLengthAttribute));
                        if (stringLengthAttr != null)
                        {
                            if (stringLengthAttr.NamedArguments.Count == 1)
                            {
                                minLenght = (int)stringLengthAttr.NamedArguments.Single(p => p.MemberName == "MinimumLength").TypedValue.Value;
                            }
                            maxLength = (int)stringLengthAttr.ConstructorArguments[0].Value;
                        }

                        var minLengthAttr = attributes.FirstOrDefault(p => p.AttributeType == typeof(MinLengthAttribute));
                        if (minLengthAttr != null)
                        {
                            minLenght = (int)minLengthAttr.ConstructorArguments[0].Value;
                        }

                        var maxLengthAttr = attributes.FirstOrDefault(p => p.AttributeType == typeof(MaxLengthAttribute));
                        if (maxLengthAttr != null)
                        {
                            maxLength = (int)maxLengthAttr.ConstructorArguments[0].Value;
                        }

                    }
                }
            }
        }
    }
}

