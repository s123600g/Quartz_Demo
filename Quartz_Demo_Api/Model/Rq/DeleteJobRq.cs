namespace Quartz_Demo_Api.Model.Rq;

using System.ComponentModel.DataAnnotations;

public class DeleteJobRq
{
    [Required]
    public string JobKey { get; set; }
}