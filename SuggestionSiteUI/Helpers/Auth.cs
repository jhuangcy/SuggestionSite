using Microsoft.AspNetCore.Components.Authorization;

namespace SuggestionSiteUI.Helpers
{
    public static class Auth
    {
        public static async Task<User> GetUser(this AuthenticationStateProvider auth, IUserService userService)
        {
            var authState = await auth.GetAuthenticationStateAsync();
            var objectId = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier"))?.Value;
            //user = await userService.FindOneFromAuthAsync(objectId);
            return await userService.FindOneFromAuthAsync(objectId);
        }
    }
}
