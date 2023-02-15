using AutoMapper;
using DAL.Entities;
using ServerAuthorization.Models.ViewModels;

namespace ServerAuthorization.Mappings
{
    public class MappingProfilesServer:Profile
	{
		public MappingProfilesServer() 
		{
			CreateMap<ServerInformation,Server >()
					.ForAllMembers(x => x.Condition(
					(src, dest, sourceValue) =>sourceValue.ToString() != "null"));
		}
	}
}
