using PetTrack.Domain.Common;

namespace PetTrack.Domain.Entities;
public class TrackerDevice : BaseEntity
{
    public string Serial { get; set; } = default!;

    public int PetId { get; set; }
    public Pet Pet { get; set; } = default!;
}