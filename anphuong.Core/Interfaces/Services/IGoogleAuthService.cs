using Google.Apis.Auth;

namespace anphuong.Core.Interfaces.Services
{
    public interface IGoogleAuthService
    {
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken);
    }
}
