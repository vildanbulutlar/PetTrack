using PetTrack.Domain.Common;
using PetTrack.Domain.Enums;

namespace PetTrack.Domain.Entities;
public class Alert : BaseEntity
{
    public int PetId { get; set; }
    public Pet Pet { get; set; } = default!;

    public AlertType Type { get; set; }
    public string Message { get; set; } = default!;
}