using KingNetwork.Shared.Encryptation;
using System.Security.Cryptography;

namespace KingNetwork.Shared.Extensions
{
    public static class RsaEncruptationExtensions
    {
        #region Extensions

        public static RsaEncryptationParameters ToRsaEncryptationParameters(this RSAParameters parameters)
        {
            return new RsaEncryptationParameters(
                parameters.Modulus,
                parameters.Exponent,
                parameters.D,
                parameters.P,
                parameters.Q,
                parameters.DP,
                parameters.DQ,
                parameters.InverseQ);
        }

        public static RSAParameters ToRSAParameters(this RsaEncryptationParameters parameters)
        {
            return new RSAParameters
            {
                Modulus = parameters.Modulus,
                Exponent = parameters.Exponent,
                D = parameters.D,
                P = parameters.P,
                Q = parameters.Q,
                DP = parameters.DP,
                DQ = parameters.DQ,
                InverseQ = parameters.IQ
            };
        }

        #endregion
    }
}
