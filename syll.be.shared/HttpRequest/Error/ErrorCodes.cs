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

        public const int DanhBaErrorNotFound = 403;
        public const int DanhBaErrorNotFoundInToChuc = 402;

        public const int FormLoaiErrorNotFound = 501;
        public const int FormLoaiErrorLayoutNotFound = 502;
        public const int FormDanhBaErrorNotFound = 503;
        public const int FormTruongDataErrorNotFound = 504;
        public const int FormLoaiErrorBlockNotFound = 505;
        public const int FormLoaiErrorBlockOrderInvalid = 506;
        public const int FormLoaiErrorRowNotFound = 507;
        public const int FormLoaiErrorRowOrderInvalid = 508;
        public const int FormLoaiErrorItemNotFound = 509;
        public const int FormLoaiErrorItemOrderInvalid = 510;
        public const int FormLoaiErrorItemTypeInvalid = 511;
        public const int FormLoaiErrorLayoutOrderInvalid = 512;

    }

}
