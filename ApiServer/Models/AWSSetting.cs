namespace ApiServer.Models
{
    public class AWSS3Settings: IAWSS3Settings
    {
        public string AccessKey {get; set;}
        public string AccessSecret {get; set;}
        public string Url {get; set;}
    }

    public interface IAWSS3Settings
    {
        string AccessKey {get; set;}
        string AccessSecret {get; set;}
        string Url {get; set;}
    }
}