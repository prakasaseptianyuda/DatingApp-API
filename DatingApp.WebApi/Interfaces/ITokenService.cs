using DatingApp.WebApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.WebApi.Interfaces
{
    public interface ITokenService
    {
        String CreateToken(User user);
    }
}
