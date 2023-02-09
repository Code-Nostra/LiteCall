using AutoMapper;
using MainServer.Models;
using MainServer.Models.ViewModels;
using MainServer.Token;
using static MainServer.Controllers.ServerController;

namespace MainServer.Mappings
{
    public class MappingProfilesServer:Profile
	{
		public MappingProfilesServer() 
		{
			CreateMap<ServerInformation,ServerDB >()
					.ForAllMembers(x => x.Condition(
					(src, dest, sourceValue) =>sourceValue.ToString() != "null"));

		}
	}
}
