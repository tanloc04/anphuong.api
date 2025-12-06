using anphuong.Core.Interfaces.Services.External;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using static anphuong.Core.Exceptions.GoogleException;

namespace anphuong.Service.Extenal
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _config;

        public GoogleAuthService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>
                {
                    _config["GOOGLE_CLIENT_ID"]
                }
                };

                return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (InvalidJwtException ex)
            {
                if (ex.Message.ToLower().Contains("expired"))
                {
                    throw new TokenExpiredException("Google token has expired.");
                }

                throw new InvalidTokenException("Invalid Google Token.");
            }
            catch (Exception)
            {
                throw new InvalidTokenException("Invalid Google Token.");
            }
        }
    }
}
