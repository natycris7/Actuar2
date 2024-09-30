using Actuar2.Dto;
using Actuar2.Models;
using AutoMapper;

namespace Actuar2.Profiles
{
    public class ProfileAutoMapper : Profile
    {
        public ProfileAutoMapper()
        {
            CreateMap<Usuario, UsuarioListarDto>();

        }

    }
}
