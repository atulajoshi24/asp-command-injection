using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

public class ReportController : Controller
{
    private readonly ILogger<ReportController> _logger;
    private readonly IWebHostEnvironment _environment;

    private static readonly Regex AlphaNumOnly = new(@"^[A-Za-z0-9]+$", RegexOptions.Compiled);


    public ReportController(
        ILogger<ReportController> logger,
        IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public IActionResult Generate(string id)
    {
        ViewData["Client"] = id;
        Console.WriteLine("id ", id);
        var process = new System.Diagnostics.Process();
        var startInfo = new System.Diagnostics.ProcessStartInfo();
        var contentPath = _environment.ContentRootPath;
        var filePath = System.IO.Path.Combine(contentPath, id);
        Console.WriteLine("file path ", filePath);
        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = $"/C echo TestOutput > {filePath}";
        process.StartInfo = startInfo;
        process.Start();

        if (process == null)
        {
            return StatusCode(500);
        }
        else
        {
            process.WaitForExit();
            return View();
        }
    }
    
    public IActionResult GenerateSafe(string? id)
    {
        if (!AlphaNumOnly.IsMatch(id))
            return BadRequest("Invalid filename. Only alphanumeric characters (A-Z, a-z, 0-9) are allowed.");
 
        ViewData["Client"] = id;
        Console.WriteLine("id ", id);
        var fileName = Path.GetFileName(id);
        var process = new System.Diagnostics.Process();
        var startInfo = new System.Diagnostics.ProcessStartInfo();
        var contentPath = _environment.ContentRootPath;
        var filePath = System.IO.Path.Combine(contentPath, id);
        Console.WriteLine("file path ", filePath);
        var fullFilePath = Path.GetFullPath(filePath);
        if (!fullFilePath.StartsWith(Path.GetFullPath(contentPath) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Invalid path");
        }
        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = "cmd.exe";
        startInfo.Arguments = $"/C echo TestOutput > {filePath}";
        process.StartInfo = startInfo;
        process.Start();

        if (process == null)
        {
            return StatusCode(500);
        }
        else
        {
            process.WaitForExit();
            return View();
        }
    }
}

//Payload : http://localhost:5049/report/generate/testReport && dir > listdir.txt