using System.Diagnostics;
using Microsoft.Playwright;

// One time setup. I would imagine we would want to look into the powershell file this runs under the hood and run it as part of Octopus deploy
// to ensure the browser engines are up to date, and maybe even cut it down to one engine like Chromium. 
// To run this locally, uncomment the following line  and comment out the rest, and run the program once. Now comment out this line and uncomment the rest 
//Microsoft.Playwright.Program.Main(["install"]);

// Default to whatever number of lines, but can be overridden by passing a number as an argument
var sizes = new List<int> { 50000 };

// Check for args. If there are any then they should be numbers that correspond to the number of lines in the test file(s) to run.
if (args.Length > 0)
{
    sizes = [];
    foreach (var arg in args)
    {
        if (int.TryParse(arg, out var lineCount))
        {
            sizes.Add(lineCount);
        }
        else
        {
            Console.WriteLine($"Invalid argument: {arg}. Must be a number.");
            return;
        }
    }
}

foreach (var lineCount in sizes)
{
    if (File.Exists($"../../../../output/statement_{lineCount}.pdf"))
    {
        File.Delete($"../../../../output/statement_{lineCount}.pdf");
    }

    var html = File.ReadAllText(@$"../../../../html/Test_html_file_{lineCount}.txt");

    var stopwatch = new Stopwatch();

    stopwatch.Start();

    Console.WriteLine($"Starting PDF generation for {lineCount} lines");
    using var playwright = await Playwright.CreateAsync();
    var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
        Headless = true
    });

    try
    {
        var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);

        await page.PdfAsync(new PagePdfOptions
        {
            Format = "A4",
            Path = $"../../../../output/statement_{lineCount}.pdf"
        });

        await page.CloseAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error generating PDF: {ex.Message}");
    }
    finally
    {
        await browser.CloseAsync();
    }

    stopwatch.Stop();

    Console.WriteLine($"{lineCount} line PDF created in {stopwatch.ElapsedMilliseconds / 1000.0}s");
}

Console.WriteLine("Press Return to close");
Console.ReadKey();