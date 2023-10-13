using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Dto;

namespace Infrastructure.Service
{
    public interface IAuthentication
    {
        public Task<bool> Login(Login login);
    }
}