using System;
using System.Collections.Generic;

namespace Menufy.API.Contracts;

public class GenerateMenuRequest
{
    public bool OverwriteExisting { get; set; } = false;
    public string? TemplateKey { get; set; } = "default";
    public Guid? TemplateId { get; set; }
    public List<string>? LanguageCodes { get; set; }
    public int? TargetCategories { get; set; }
    public int? TargetItemsPerCategory { get; set; }
}

