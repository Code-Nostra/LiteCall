using AutoMapper;
using DAL.Entities;
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
			CreateMap<ServerInformation,Server >()
					.ForAllMembers(x => x.Condition(
					(src, dest, sourceValue) =>sourceValue.ToString() != "null" && sourceValue!= null));

		}
	}
}
