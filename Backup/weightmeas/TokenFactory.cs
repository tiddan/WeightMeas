using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace weightmeas
{
    public class TokenFactory
    {
        private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GenerateToken()
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var buffer = new char[10];
            for(var i=0;i<10;i++)
            {
                buffer[i] = Characters[random.Next(Characters.Length)];
            }
            return new string(buffer);
        }
    }
}