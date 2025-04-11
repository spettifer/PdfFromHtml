// See https://aka.ms/new-console-template for more information

using System.Text;

const int numberOfLines = 40000;
var fileName = $"Test_html_file_{numberOfLines}.txt";
var filePath = "D:\\_git\\PDF\\html";
var fragmentsFolder = "D:\\_git\\PDF\\HtmlGenerator\\Fragments";

// Define the order of fragments
var fragmentFiles = new[] { "Head.txt", "OpeningBody.txt" };

try
{
    // Combine the contents of the fragments
    var combinedContent = string.Empty;
    foreach (var fragmentFile in fragmentFiles)
    {
        var fragmentPath = Path.Combine(fragmentsFolder, fragmentFile);
        if (File.Exists(fragmentPath))
        {
            combinedContent += File.ReadAllText(fragmentPath);
        }
        else
        {
            Console.WriteLine($"Fragment file not found: {fragmentPath}");
        }
    }
    
    combinedContent += HtmlMainTable(numberOfLines);
    
    var closingFragmentPath = Path.Combine(fragmentsFolder, "ClosingBody.txt");
    
    if (File.Exists(closingFragmentPath))
    {
        combinedContent += File.ReadAllText(closingFragmentPath);
    }
    else
    {
        Console.WriteLine($"Fragment file not found: {closingFragmentPath}");
    }
    
    // Write the combined content to the output file
    var outputFilePath = Path.Combine(filePath, fileName);
    File.WriteAllText(outputFilePath, combinedContent);

    Console.WriteLine($"Combined file written to: {outputFilePath}");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

return;


string HtmlMainTable(int numberOfLines)
{
    const string tableContentHeader =
        "<div class=\"instructions\"><p class=\"heading\">Instructions (25)</h3><table class=\"instructions-table\"><thead><tr><th>Reference</th><th>Payee Details</th><th>Requested Execution Date Time</th><th>Amount</th><th>Status</th></tr></thead><tbody>";
    
        var builder = new StringBuilder();

        for (var i = 0; i < numberOfLines; i++)
        {
            builder.Append(
                "<tr><td>SalaryNovember</td><td>John Smith <span class=\"numeric-spacing\">12345671</span> <span class=\"numeric-spacing\">123451</span></td><td>16 Nov 2023</td><td>100</td><td>Awaiting approval</td></tr>");
        }
        builder.Append("</tbody></table></div>");
        
        return builder.ToString();
}