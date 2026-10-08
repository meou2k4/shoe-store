using AppData.DTO;
using AppData.Models;
using AutoMapper;

namespace AppAPI
{
	public class MappingProfile: Profile
	{
        public MappingProfile()
        {
			CreateMap<GiamgiaDTO, Giamgia>().ReverseMap();
		}
    }
}
