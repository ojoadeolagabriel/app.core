using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using app.core.data.dao;
using app.core.data.dto;
using app.core.exception.management;
using app.core.exception.management.code;
using app.core.security.otp;
using app.core.util.reflection;
using appcore.test.dao;
using appcore.test.dto;

namespace appcore.test
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = new TokenRequestor
            {
                CreatedOn = DateTime.Now,
                Name = "WebPAY_" + DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture)
            };

            //create requestor
            var currentRequestor = TokenManager.CreateTokenRequestor(data);

            //register my token
            var token = new Token
            {
                ChildKey = TokenManager.Get10DigitByteBuffer(),
                ExpirationDate = DateTime.Now.AddMinutes(1),
                Counter = 1,
                Requestor = data,
                TokenKey = "07061890552",
                LastOneTimePassword = ""
            };

            var tokenDao = new TokenDao("");
            tokenDao.Persist(token);

            //generate otp
            var totalBuffer = new byte[20];
            var childKey = token.ChildKey;
            System.Buffer.BlockCopy(currentRequestor.ParentKey, 0, childKey, 0, currentRequestor.ParentKey.Length);

            var otp = new OTP((ulong)token.Counter, totalBuffer);
            var otpValue = otp.GetCurrentOtp();

            token.LastOneTimePassword = otpValue;
            token.ExpirationDate = DateTime.Now.AddMinutes(2);
            tokenDao.Update(token);
        }
    }

    public class TokenManager
    {
        public static byte[] Get10DigitByteBuffer()
        {
            var randomBuffer = new byte[10];
            new Random().NextBytes(randomBuffer);
            return randomBuffer;
        }

        public static TokenRequestor CreateTokenRequestor(TokenRequestor requestor)
        {
            if (requestor != null)
            {
                //confirm if requestor exists
                var dao = new TokenRequestorDao("");
                var existingRequestor = dao.RetreiveById(requestor.Id);
                if (existingRequestor != null)
                    ExceptionHandler.Handle("Token requestor already exists", ExceptionCode.RequestorAllReadyExists);


                requestor.ParentKey = Get10DigitByteBuffer();
                dao.Persist(requestor);

                return requestor;
            }

            return null;
        }
    }
}
