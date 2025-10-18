using PetTrack.Domain.Common;

namespace PetTrack.Domain.Entities;
public class VetAppointment : BaseEntity
{
    public int PetId { get; set; }
    public Pet Pet { get; set; } = default!;
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string Title { get; set; } = "";
}