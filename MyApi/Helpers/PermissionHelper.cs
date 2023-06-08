using System.Linq;
using System.Security.Claims;
using Common.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyApi.Helpers
{
    public class PkPermissionAttribute : TypeFilterAttribute
    {
        public PkPermissionAttribute(ResourcesEnum resources) : base(typeof(PkPermissionAuthorize))
        {
            Arguments = new object[] { resources };
        }
    }

    public class PkPermissionAuthorize : IAuthorizationFilter
    {
        private readonly ResourcesEnum _resources;

        public PkPermissionAuthorize(ResourcesEnum input)
        {
            _resources = input;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var roles = context.HttpContext.User.FindAll(c => c.Type == ClaimTypes.Role);
            if (roles == null || roles.All(c => c.Value != _resources.ToString()))
            {
                context.Result = new ForbidResult();
            }
        }

        //public void OnAuthorization(AuthorizationFilterContext context)
        //{
        //    var strResourceIds = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
        //    if (string.IsNullOrEmpty(strResourceIds))
        //    {
        //        context.Result = new ForbidResult();
        //    }
        //    else
        //    {
        //        var resource = (int)_resources;
        //        var resourceIds = strResourceIds.Split(',');

        //        if (resourceIds.All(c => c != resource.ToString()))
        //        {
        //            context.Result = new ForbidResult();
        //        }
        //    }
        //}
    }
    //public class PkPermissionAttribute : TypeFilterAttribute
    //{
    //    public PkPermissionAttribute(ResourcesEnum resources) : base(typeof(PkPermissionAuthorize))
    //    {
    //        Arguments = new object[] { resources };
    //    }
    //}

    //********************************************************************************************************************
    // public class PkPermissionAuthorize : IAuthorizationFilter
    // {
    //private readonly ResourcesEnum _resources;

    //public PkPermissionAuthorize(ResourcesEnum input)
    //{
    //    _resources = input;
    //}

    //public void OnAuthorization(AuthorizationFilterContext context)
    //{
    //    var strResourceIds = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
    //    if (string.IsNullOrEmpty(strResourceIds))
    //    {
    //        context.Result = new ForbidResult();
    //    }
    //    else
    //    {
    //        var resource = (int)_resources;
    //        var resourceIds = strResourceIds.Split(',');

    //        if (resourceIds.All(c => c != resource.ToString()))
    //        {
    //            context.Result = new ForbidResult();
    //        }
    //    }
    //}
    // }
}