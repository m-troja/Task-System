using Task_System.Model.Entity;

namespace Task_System.Model.DTO.Cnv
{
    public class UserCnv
    {
        public UserDto ConvertUserToDto(User user)
        {
            return new UserDto(
                user.FirstName, 
                user.LastName, 
                user.Email, 
                user.Roles.Select(r => r.Name).ToList(), 
                user.Teams.Select(t => t.Name).ToList()
            );
        }

        public List<UserDto> ConvertUsersToUsersDto(List<User> users)
        {
            List<UserDto> userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(ConvertUserToDto(user));
            }
            return userDtos;
        }
    }
}
