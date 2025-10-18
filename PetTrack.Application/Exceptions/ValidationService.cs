using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetTrack.Application.Validation;
public static class ValidationService
{
    public static void NotEmpty(string? v, string f) { if (string.IsNullOrWhiteSpace(v)) throw new Exceptions.ValidationException($"{f} boş olamaz!"); }
    public static void Positive(int v, string f) { if (v <= 0) throw new Exceptions.ValidationException($"{f} pozitif olmalı!"); }
}
