namespace TimeBasedOTP
{
    /// <summary>
    /// To support multiple Hash algorithm
    /// </summary>
    public interface IHashAlgorithm
    {
        System.Type Algorithm { get; }
        byte[] HashingAMessageWithASecretKey(byte[] iterationNumberByte, byte[] userIdByte);
    }
}
