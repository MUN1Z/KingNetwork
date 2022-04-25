using System.Numerics;

namespace KingNetwork.Shared.Encryptation
{
    public class RsaEncryptationParameters
    {
        #region poperties

        public byte[] Modulus { get; set; }
        public byte[] Exponent { get; set; }
        public byte[] D { get; set; }
        public byte[] P { get; set; }
        public byte[] Q { get; set; }
        public byte[] DP { get; set; }
        public byte[] DQ { get; set; }
        public byte[] IQ { get; set; }

        public bool IsPublicKey => D == null || D.Length == 0;

        #endregion

        #region constructors

        public RsaEncryptationParameters() { }

        public RsaEncryptationParameters(byte[] modulus, byte[] exponent)
        {
            Modulus = modulus;
            Exponent = exponent;
        }


        public RsaEncryptationParameters(BigInteger modulus, BigInteger exponent)
            : this(modulus.ToByteArray(), exponent.ToByteArray()) { }

        public RsaEncryptationParameters(string modulus, string exponent)
            : this(BigInteger.Parse(modulus), BigInteger.Parse(exponent)) { }

        public RsaEncryptationParameters(
            byte[] modulus,
            byte[] exponent,
            byte[] d,
            byte[] p,
            byte[] q,
            byte[] dp,
            byte[] dq,
            byte[] iq)
        {
            Modulus = modulus;
            Exponent = exponent;
            D = d;
            P = p;
            Q = q;
            DP = dp;
            DQ = dq;
            IQ = iq;
        }

        public RsaEncryptationParameters(
            BigInteger modulus,
            BigInteger exponent,
            BigInteger d,
            BigInteger p,
            BigInteger q,
            BigInteger dp,
            BigInteger dq,
            BigInteger iq) : this(
            modulus.ToByteArray(),
            exponent.ToByteArray(),
            d.ToByteArray(),
            p.ToByteArray(),
            q.ToByteArray(),
            dp.ToByteArray(),
            dq.ToByteArray(),
            iq.ToByteArray()) { }

        public RsaEncryptationParameters(
            string modulus,
            string exponent,
            string d,
            string p,
            string q,
            string dp,
            string dq,
            string iq) : this (
            BigInteger.Parse(modulus),
            BigInteger.Parse(exponent),
            BigInteger.Parse(d),
            BigInteger.Parse(p),
            BigInteger.Parse(q),
            BigInteger.Parse(dp),
            BigInteger.Parse(dq),
            BigInteger.Parse(iq)) { }

        #endregion
    }
}
