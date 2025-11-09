// ABOUTME: Shared helper methods for formatting nutritional information with consistent ACTUAL/TARGET labeling across console and Todoist outputs

namespace SystemOfEquations;

internal static class NutritionalFormatting
{
    /// <summary>
    /// Formats nutritional information with optional ACTUAL/TARGET labels.
    /// When hasConversionFoods is true, shows ACTUAL and TARGET labels.
    /// Otherwise, shows just the actual value without labels.
    /// </summary>
    /// <param name="actual">The actual nutritional information to display</param>
    /// <param name="target">The target macros (optional, only used when hasConversionFoods is true)</param>
    /// <param name="hasConversionFoods">Whether conversion foods are present (determines labeling)</param>
    /// <param name="prefix">Prefix for each line (e.g., "  " for indentation)</param>
    /// <param name="separator">Separator between ACTUAL and TARGET (default is "\n")</param>
    /// <returns>Formatted string with appropriate labeling</returns>
    public static string FormatWithOptionalTarget(
        string actual,
        string? target,
        bool hasConversionFoods,
        string prefix = "",
        string separator = "\n")
    {
        if (hasConversionFoods && target != null)
        {
            return $"{prefix}ACTUAL: {actual}{separator}{prefix}TARGET: {target}";
        }
        else
        {
            return $"{prefix}{actual}";
        }
    }
}
