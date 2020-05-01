using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wapiSeg1BD.Models;

namespace wapiSeg1BD.Interfaces
{
    public interface IHashService
    {
        HashResult Hash(string input);
        HashResult Hash(string input, byte[] salt);
    }
}
