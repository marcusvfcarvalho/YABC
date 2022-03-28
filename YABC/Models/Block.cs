using System.ComponentModel.DataAnnotations.Schema;

namespace YABC.Models;

public record Block
{
    public int Id { get; set; } 

    public string Name { get; set; } = string.Empty;

    public DateTime CreationDateTime { get; set; } = DateTime.Now;

    public Person? Person { get; set; }

    [ForeignKey("Person")]
    public int PersonId { get; set; }
    
    public byte[] Image { get; set; } = Array.Empty<byte>();

    public string Hash256 { get; set; } = string.Empty;

    public double Nonce { get; set; } 

    public string PreviousHash256 { get; set; } = string.Empty;
}
