namespace Quartz_Demo_Api.Model.Rq;

using System.ComponentModel.DataAnnotations;

public class AddJobRq
{
    [Required]
    public string JobKey { get; set; }
    
    public string? GroupName { get; set; }
}