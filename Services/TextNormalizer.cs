namespace WikipediaAutomationProject.Services;

using System.Text.RegularExpressions;

public class TextNormalizer
{
    public static string NormalizeText(string text)
    {
        // Remove brackets and their contents (e.g., [4])
        var withoutBrackets = Regex.Replace(text,  @"\[.*?\]", string.Empty);
        // Replace delimiters (periods, hyphens, commas, etc.) with spaces
        var delimiterSpaces = Regex.Replace(withoutBrackets, @"[^\w\s]", " ");
        // Convert to lowercase
        return delimiterSpaces.ToLower();
    }
}