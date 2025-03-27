using System;
using System.IO;
using System.Reflection;

namespace Demo.App.Shared.Prompts;

public class PromptLoader : IPromptLoader
{
    private string Folder { get; set; }

    public PromptLoader()
    {
        Folder = AppDomain.CurrentDomain.BaseDirectory;
    }

    public string LoadPrompt(string path)
    {
        var fullPath = Path.Combine(Folder, path);
        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"The prompt file was not found at: {fullPath}");

        return File.ReadAllText(fullPath);
    }
}

