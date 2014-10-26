using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using app.core.data.dao;
using app.core.data.dto;
using app.core.exception.management;
using app.core.exception.management.code;
using app.core.security.otp;
using app.core.util.reflection;


namespace appcore.test
{
    class AsyncFactory
    {
        public static Task<string> Pro()
        {
            var content = new StringContent(@"yello bhan me yello bhan me yello bhan me yello bhan me yello bhan me");
            return content.ReadAsStringAsync();
        }

        public static Task<int> GetIntAsync()
        {
            var tcs = new TaskCompletionSource<int>();
            var timer = new Timer(2000) { AutoReset = false };
            timer.Elapsed += (s, e) =>
            {
                tcs.SetResult(10);
                timer.Dispose();
            };

            timer.Start();
            return tcs.Task;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var watch = new Stopwatch();
            watch.Start();

            var userDao = new UserDao("");
            for (var i = 0; i < 1000; i++)
            {
                var userInst = userDao.GetUserInstituion(1);
                var user = userDao.RetreiveById(1);
                var secondUser = userDao.GetSecondUser(1);

                Console.WriteLine("iteration: [ {0} ]", i);
            }
            var totalTime = watch.Elapsed;
            watch.Reset();
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

        public static Token CreateToken(Token token, TokenRequestor tokenRequestor)
        {
            var tokenDao = new TokenDao("");
            tokenDao.Persist(token);

            //generate otp
            var totalBuffer = new byte[20];
            var childKey = token.ChildKey;
            System.Buffer.BlockCopy(tokenRequestor.ParentKey, 0, childKey, 0, tokenRequestor.ParentKey.Length);

            var otp = new OTP((ulong)token.Counter, totalBuffer);
            var otpValue = otp.GetCurrentOtp();

            token.LastOneTimePassword = otpValue;
            token.ExpirationDate = DateTime.Now.AddMinutes(2);
            tokenDao.Update(token);

            return token;
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
