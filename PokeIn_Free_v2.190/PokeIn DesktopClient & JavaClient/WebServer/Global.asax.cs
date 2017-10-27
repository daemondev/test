using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using PokeIn.Comet;

namespace WebServer
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            //automatically encodes and decodes the messages using Base64 Encoder (suggested)
            CometSettings.Base64EncyrptedTransfers = true;

            CometWorker.OnClientConnected += new DefineClassObjects(CometWorker_OnClientConnected);
        }

        static void CometWorker_OnClientConnected(ConnectionDetails details, ref Dictionary<string, object> classList)
        {
            //all the ServerInstance class instance methods (public void) will be accessible from your clients
            classList.Add("MyServer", new ServerInstance(details.ClientId, details.IsDesktopClient));
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}