using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PetTrack.Domain.Common;
using PetTrack.Domain.Enums;
namespace PetTrack.Domain.Entities;
public class Pet : BaseEntity
{
    public string Name { get; set; } = null!;
    public string SpeciesName { get; set; } = null!;
    public PetFamily Family { get; set; }
    public int OwnerId { get; set; }
    public virtual PetOwner Owner { get; set; } = null!;
    public TrackerDevice? Device { get; set; }
}