using MyShop_Site.Models.RequestModels;
using MyShop_Site.Models.ResponseModels;

namespace MyShop_Site.Models.Authentication
{

    public class CreateUserModel : JsonRequestModel
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
    public class LoginUserModel : JsonRequestModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    //public class CreateUserResponseModel : IResponseModel
    //{
    //    public Guid ID { get; set; }
    //}
    //public class LoginUserResponseModel : IResponseModel
    //{
    //    public string Token{ get; set; }
    //}
 
    public class VerifyUserModel : JsonRequestModel
    {
        public Guid ID { get; set; }
        public string OtpCode { get; set; }
    }
    //public class VerifyUserResponseModel : IResponseModel
    //{
    //    public Guid ID { get; set; }
    //    public string UserPassword { get; set; }
    //}
    public class SetUserPasswordModel : JsonRequestModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}

