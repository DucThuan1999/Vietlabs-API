namespace VietLab.Models.DTOs;

public class MeRequest
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}

public class ReplaceModuleGrantsRequest
{
    public List<ModuleGrantItemDto> Grants { get; set; } = new();
}

public class ModuleGrantItemDto
{
    public string ModuleCode { get; set; } = string.Empty;
    public string ActionCode { get; set; } = string.Empty;
}
