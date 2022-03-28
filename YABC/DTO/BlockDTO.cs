namespace YABC.DTO;

public class BlockDTO
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreationDateTime { get; set; } = DateTime.Now;

    public int PersonId { get; set; }

    public string Hash256 { get; set; } = string.Empty;

    public double Nonce { get; set; }

    public string PreviousHash256 { get; set; } = string.Empty;
}
