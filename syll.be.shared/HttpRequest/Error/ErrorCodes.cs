using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.shared.HttpRequest.Error
{
    public static class ErrorCodes
    {
        public const int System = 1;
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int NotFound = 404;
        public const int Found = 409;
        public const int InternalServerError = 500;


        public const int AuthInvalidPassword = 101;
        public const int AuthErrorCreateUser = 102;
        public const int AuthErrorUserNotFound = 103;
        public const int AuthErrorCreateRole = 104;
        public const int AuthErrorRoleNotFound = 105;
        public const int AuthErrorUserEmailHuceNotFound = 106;

        public const int ToChucErrorNotFound = 201;
        public const int ToChucErrorLoaiToChucNotFound = 202;


        public const int ServiceAccountErrorNotFound = 301;
        public const int GoogleSheetUrlErrorInvalid = 302;
    }
}
