using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Carly.Api.Features.Shared.Models;

namespace Carly.Api.Controllers.v1.Contracts.Request;

public sealed class RegisterBookingRequest
{
    [Required]
    public CarCategory? Category { get; init; }
    
    [Required, DefaultValue("900325-1234")]
    public string? CustomerId { get; init; }
}
