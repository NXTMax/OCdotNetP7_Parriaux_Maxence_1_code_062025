using P7CreateRestApi.Models;

namespace P7CreateRestApi.Dtos
{
    public class UserDisplayDto()
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
