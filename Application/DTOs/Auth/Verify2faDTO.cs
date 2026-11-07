namespace Application.DTOs.Auth;

[System.Text.Json.Serialization.JsonUnmappedMemberHandling(System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow)]
public class Verify2FaDTO
{
    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.StringLength(43, MinimumLength = 43)]
    public string ChallengeToken { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required]
    [System.ComponentModel.DataAnnotations.RegularExpression("^[0-9]{6}$")]
    public string Code { get; set; } = string.Empty;
}
