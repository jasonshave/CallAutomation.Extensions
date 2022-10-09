using System.ComponentModel.DataAnnotations;
using Azure.Communication;

namespace CallAutomation.Extensions.Models;

public sealed class DtmfOptions
{
    [Required]
    public CommunicationIdentifier Target { get; set; }

    [Required]
    public int TonesToCollect { get; set; }

}