using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EmployeeAudit
{
    public static class Helper
    {
        public static Dictionary<string, string> ConvertStringArrayToDictionary(IReadOnlyList<string> args)
        {
            var hasDuplicates = args.Where(s => s.StartsWith("--")).GroupBy(x => x).Any(g => g.Count() > 1);
            if (hasDuplicates)
            {
                throw new ArgumentException("Invalid input: Has duplicate parameter/-s");
            }
            
            var elements = string.Join(" ", args).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (elements.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid input: Missing parameter value");
            }
            
            var parameters = elements
                .Select((s, i) => new { Value = s, Index = i })
                .GroupBy(x => x.Index / 2, x => x.Value)
                .ToDictionary(g => g.First(), g => g.Skip(1).First());

            return parameters;
        }
        
        public static class ObjectValidator<T> where T : class
        {
            public static List<ValidationResult> Validate(T obj, IEnumerable<string> properties)
            {
                var validationResults = new List<ValidationResult>();

                foreach (var property in properties)
                {
                    var validationContext = new ValidationContext(obj, serviceProvider: null, items: null);
                    var propertyValue = typeof(T).GetProperty(property)?.GetValue(obj);
                    
                    validationContext.MemberName = property;
                    
                    if (propertyValue == null)
                    {
                        
                        validationResults.Add(new ValidationResult($"Property {property} cannot be null", new[] { validationContext.MemberName }));
                        continue;
                    }
                
                    Validator.TryValidateProperty(
                        propertyValue,
                        validationContext,
                        validationResults);
                }
                
                return validationResults;
            }
        }

        public static void ErrorMessage(IEnumerable<ValidationResult> validationResults)
        {
            var message = string.Join(Environment.NewLine, validationResults);
            throw new ArgumentException(message);
        }
    }
}