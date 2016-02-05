using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace Webforms.Samples.Manage
{
    public partial class AddPhoneNumber : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void PhoneNumber_Click(object sender, EventArgs e)
        {
            var manager = Context.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // Send result of: manager.GetPhoneNumberCodeAsync(User.Identity.GetUserId(), phoneNumber);
            // Generate the token and send it
            var code = manager.GenerateChangePhoneNumberToken(User.Identity.GetUserId(), PhoneNumber.Text);
            if (manager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = PhoneNumber.Text,
                    Body = "Your security code is: " + code
                };

                Task.Run(async () => { await manager.SmsService.SendAsync(message); });
            }

            Response.Redirect("/Account/VerifyPhoneNumber?PhoneNumber=" + PhoneNumber.Text);
        }
    }
}