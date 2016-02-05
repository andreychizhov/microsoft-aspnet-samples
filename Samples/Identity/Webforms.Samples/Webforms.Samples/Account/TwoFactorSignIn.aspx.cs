using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Web;

namespace Webforms.Samples.Account
{
    public partial class TwoFactorSignIn : System.Web.UI.Page
    {
        private SignInHelper signinHelper;
        private ApplicationUserManager manager;

        public TwoFactorSignIn()
        {
            manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            signinHelper = new SignInHelper(manager, Context.GetOwinContext().Authentication);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var userId = signinHelper.GetVerifiedUserId();
            if (userId == null)
            {
                Response.Redirect("/Account/Error", true);
            }
            var userFactors = manager.GetValidTwoFactorProviders(userId);
            Providers.DataSource = userFactors.Select(x => x).ToList();
            Providers.DataBind();
        }

        protected void CodeSubmit_Click(object sender, EventArgs e)
        {
            var result = signinHelper.TwoFactorSignIn(SelectedProvider.Value, Code.Text, isPersistent: false, rememberBrowser: RememberBrowser.Checked);
            switch (result)
            {
                case SignInStatus.Success:
                    IdentityHelper.RedirectToReturnUrl(Request.QueryString["ReturnUrl"], Response);
                    break;
                case SignInStatus.LockedOut:
                    Response.Redirect("/Account/Lockout");
                    break;
                case SignInStatus.Failure:
                default:
                    FailureText.Text = "Invalid code";
                        ErrorMessage.Visible = true;
                    break;
            }
        }

        protected void ProviderSubmit_Click(object sender, EventArgs e)
        {
            if (!signinHelper.SendTwoFactorCode(Providers.SelectedValue))
            {
                Response.Redirect("/Account/Error");
            }

            var user = manager.FindById(signinHelper.GetVerifiedUserId());
            if (user != null)
            {
                // To exercise the flow without actually sending codes, uncomment the following line
                DemoText.Text = "For DEMO purposes the current " + Providers.SelectedValue + " code is: " + manager.GenerateTwoFactorToken(user.Id, Providers.SelectedValue);
            }

            SelectedProvider.Value = Providers.SelectedValue;
            sendcode.Visible = false;
            verifycode.Visible = true;
        }
    }
}