using P7CreateRestApi.Dtos;
using P7CreateRestApi.Models;

namespace P7CreateRestApi.Mappers
{
    public static class UserMapper
    {
        public static UserDisplayDto ToUserDisplayDto(this User user) => new UserDisplayDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = user.FullName,
            Email = user.Email
        };

        public static IEnumerable<UserDisplayDto> ToUserDisplayDtoEnumerable(this IEnumerable<User> users) =>
            users.Select(user => user.ToUserDisplayDto());
    }
}
