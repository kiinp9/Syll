using syll.be.shared.HttpRequest.AppException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace syll.be.shared.HttpRequest.AppException
{
    public class UserFriendlyException : BaseException
    {
        public UserFriendlyException(int errorCode) : base(errorCode)
        {
        }

        public UserFriendlyException(int errorCode, string? messsage) : base(errorCode, messsage)
        {
        }
    }
}