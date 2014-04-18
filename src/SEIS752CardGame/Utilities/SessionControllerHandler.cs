using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace SEIS752CardGame.Utilities
{
    public class SessionControllerHandler : HttpControllerHandler, IRequiresSessionState
    {
        public SessionControllerHandler(RouteData routeData) : base(routeData)
        { }
    }
}