using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetTrack.Application.Exceptions;
public class ValidationException : Exception { public ValidationException(string m) : base(m) { } }
