using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PetTrack.Domain.Common;
namespace PetTrack.Domain.Entities;
public class PetOwner : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}
