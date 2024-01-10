using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace dotnet_core_auth_api.Models.Users
{
    public class UserProfile
    {
        [Key]
        public string UserId { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
    }
}

/*
 {
  "firstname": "Will",
  "lastname": "Gale",
  "age": 23,
  "sex": "Male",
  "height": 6.2,
  "weight": 148,
  "userId": "fc21de8c-6810-4071-8411-c0498c92dbfd"
}
*/