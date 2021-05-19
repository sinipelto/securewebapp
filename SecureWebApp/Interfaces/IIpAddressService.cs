namespace SecureWebApp.Interfaces
{
    public interface IIpAddressService
    {
        string GetRequestIp(bool tryUseXForwardHeader = true);
 
        T GetHeaderValueAs<T>(string headerName);
    }
}