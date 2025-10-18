using PetTrack.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PetTrack.Application.DTOs.Pets;
public record CreatePetDto(string Name, string SpeciesName, PetFamily Family, int OwnerId);
