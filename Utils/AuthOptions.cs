using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace webFerum.Utils
{
    public class AuthOptions
    {
        public const string ISSUER = "webfer"; 
        public const string AUDIENCE = "wfclient"; 
        const string KEY = "mysupersecret_secretsecretsecretkeyeebn";   
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
