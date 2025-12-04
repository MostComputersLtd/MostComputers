using FluentValidation.Results;
using System.Text.RegularExpressions;

namespace MOSTComputers.UI.Web.Blazor.Utils;

internal static partial class ValidationResultToHierarchyConversion
{
    internal sealed class ValidationErrorNode
    {
        public required string PropertyName { get; set; }
        public required string ErrorCode { get; set; }
        public required string FullPropertyPath { get; set; }
        public List<string> Errors { get; set; } = new();

        private readonly List<ValidationErrorNode> _children = new();
        public IReadOnlyList<ValidationErrorNode> Children => _children;

        public ValidationErrorNode? GetChildByPropertyName(string propertyName)
        {
            return _children.FirstOrDefault(c => c.PropertyName == propertyName);
        }

        public void TryAddChild(ValidationErrorNode child)
        {
            ValidationErrorNode? existingChild = _children.FirstOrDefault(c => c.PropertyName == child.PropertyName);

            if (existingChild is null)
            {
                _children.Add(child);
            }
        }
    }

    [GeneratedRegex(@"\[(\d+)\]", RegexOptions.Compiled)]
    private static partial Regex ArrayRegex();

    //internal static ValidationErrorNode GetHierarchy(List<ValidationFailure> validationFailures)
    //{
         
    //}

    private static List<string> SplitPropertyPath(string propertyPath)
    {
        if (string.IsNullOrEmpty(propertyPath))
        {
            return new();
        }

        List<string> result = new();

        int index = 0;

        foreach (string part in propertyPath.Split('.'))
        {
            Match match = ArrayRegex().Match(part);

            if (match.Success)
            {
                string propName = part[..match.Index];

                if (!string.IsNullOrEmpty(propName))
                {
                    result[index++] = propName;
                }

                result[index++] = match.Groups[1].Value;
            }
            else
            {
                result[index++] = part;
            }
        }

        return result;
    }
}