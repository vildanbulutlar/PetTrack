using PetTrack.Domain.Common;

namespace PetTrack.Domain.Entities;
public class HealthRecord : BaseEntity
{
    public int PetId { get; set; }
    public Pet Pet { get; set; } = default!;

    public string Title { get; set; } = "";      // Aşı / Muayene / Alerji vb.
    public string? Notes { get; set; }           // Detay
    public DateTime RecordDateUtc { get; set; }  // Kayıt tarihi (UTC)
}