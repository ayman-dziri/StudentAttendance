using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using StudentAttendanceV2.src.StudentAttendance.Application.Interfaces;

namespace StudentAttendanceV2.src.StudentAttendance.Application.Services;

    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor ;
        private const string UserIdClaim = JwtRegisteredClaimNames.Sub;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }



        public string GetCurrentUserId()
        {


            var context = _httpContextAccessor.HttpContext;
            


            if(context?.User?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var userId = context.User.FindFirst(UserIdClaim)?.Value; //ce qui est concerné par le token ("lid de lutilisateur")
            //sub ---> lutilisateur
            //aud ---> L api qui doit accepter le token ---> StudentAttendanceAPI
            //iss ---> qui a émis le token
                        
            
            if(string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID claim missing");
            }
           
                return userId;
            
        }
    }
