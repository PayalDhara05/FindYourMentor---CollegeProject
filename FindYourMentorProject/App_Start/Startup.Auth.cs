using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using FindYourMentorProject.Models;
using Microsoft.Owin.Security;

namespace FindYourMentorProject
{
    public partial class Startup
    {
        private void ConfigureAuth(IAppBuilder app)
        {
            var cookieOptions = new CookieAuthenticationOptions
            {
                LoginPath = new PathString("/User/Login")
            };

            app.UseCookieAuthentication(cookieOptions);


            app.SetDefaultSignInAsAuthenticationType(cookieOptions.AuthenticationType);

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "613050669339-l4hnu2eji5a7epqp9skbnp4hoc678777.apps.googleusercontent.com",
                ClientSecret = "RpK1NgT-T9aDtArPI7-eAECQ"
            });
        }

        // Uncomment the following lines to enable logging in with third party login providers
        //app.UseMicrosoftAccountAuthentication(
        //    clientId: "",
        //    clientSecret: "");

        //app.UseTwitterAuthentication(
        //   consumerKey: "",
        //   consumerSecret: "");

        //app.UseFacebookAuthentication(
        //   appId: "",
        //   appSecret: "");

        //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
        //{
        //    ClientId = "613050669339-l4hnu2eji5a7epqp9skbnp4hoc678777.apps.googleusercontent.com",
        //    ClientSecret = "RpK1NgT-T9aDtArPI7-eAECQ"
        //});    
    }
}