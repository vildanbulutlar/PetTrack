using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PetTrack.Domain.Entities;
namespace PetTrack.Domain.Abstractions;
public interface IPetOwnerRepository : IRepository<PetOwner> { }
