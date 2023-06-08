using Common.Enum;
using Common.Helper;
using DataAccess.Interface.Security;
using DataModel.Models;
using DataModel.ViewModel.Role;
using Microsoft.AspNetCore.Mvc;
using MyApi.Controllers.Common;
using MyApi.Helpers;

namespace MyApi.Controllers.Security;

[Route("api/[controller]")]
[ApiController]
public class RoleController : BaseController
{
    #region constructor
    private readonly IRoleDal _role;
    private readonly IResourcesDal _resources;
    private readonly IRolePolicyDal _rolePolicy;

    public RoleController(IRoleDal role, IResourcesDal resources, IRolePolicyDal rolePolicy)
    {
        _role = role;
        _resources = resources;
        _rolePolicy = rolePolicy;
    }
    #endregion

    #region Get Role
    [HttpGet]
    [Route("GetRoleList")]
    [PkPermission(ResourcesEnum.RoleManagement)]
    public async Task<ActionResult<List<Role>>> GetRoleList()
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }
            #endregion

            #region Get Role List
            var roles = await _role.GetList();

            return roles;
            #endregion
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region Add Role
    [HttpPost]
    [Route("AddRole")]
    [PkPermission(ResourcesEnum.RoleManagement)]
    public async Task<ActionResult<Role>> AddRole(Role data)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (string.IsNullOrEmpty(data.Title?.Trim()))
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Insert Role
            var roleModel = new Role
            {
                Title = data.Title.Trim(),
                Comment = data.Comment?.Trim(),
                CreatorId = UserId,
                CreateOn = DateTime.Now
            };

            var roleId = await _role.Insert(roleModel);

            if (roleId <= 0) return null;

            roleModel.Id = roleId;

            return roleModel;
            #endregion
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region Edit Role
    [HttpPost]
    [Route("EditRole")]
    [PkPermission(ResourcesEnum.RoleManagement)]
    public async Task<ActionResult<Role>> EditRole(Role data)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (data.Id <= 0 || string.IsNullOrEmpty(data.Title.Trim()))
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Edit Role

            var roleModel = await _role.GetById(data.Id);

            roleModel.Title = data.Title.Trim();
            roleModel.Comment = data.Comment.Trim();

            var roleId = await _role.Update(roleModel);

            if (roleId <= 0) return null;

            return roleModel;
            #endregion
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region GetResourses
    [HttpGet]
    [Route("GetResourcesList")]
    [PkPermission(ResourcesEnum.RoleManagement)]
    public async Task<ActionResult<List<Resources>>> GetResourcesList()
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }
            #endregion

            #region Get Resources List
            var resources = await _resources.GetList();

            return resources;
            #endregion
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region GetAccessibleResources
    [HttpGet]
    [Route("GetAccessibleResources/{roleId}")]
    [PkPermission(ResourcesEnum.RoleManagement)]
    public async Task<ActionResult<List<AccessibleResourceViewModel>>> GetAccessibleResources(long roleId)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (roleId <= 0)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Get AccessibleResources
            var resources = await _resources.GetList();
            var rolePolicy = await _rolePolicy.GetByRoleId(roleId);

            return resources.Select(c => new AccessibleResourceViewModel
            {
                ResourceId = c.Id,
                ResourceKey = c.ResourceKey,
                ResourceName = c.ResourceName,
                IsAccess = rolePolicy.Any(rp => rp.ResourceId == c.Id)
            }).ToList();
            #endregion
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion

    #region SetRolePolicy
    [HttpPost]
    [Route("SetRolePolicy")]
    [PkPermission(ResourcesEnum.RoleManagement)]
    public async Task<ActionResult<bool>> SetRolePolicy(SetRolePolicyViewModel data)
    {
        try
        {
            #region Validation
            if (UserId <= 0)
            {
                return HttpHelper.AccessDeniedContent();
            }

            if (data.RoleId <= 0 || !data.ResourceIds.Any())
            {
                return HttpHelper.InvalidContent();
            }

            var role = await _role.GetById(data.RoleId);
            if (role == null)
            {
                return HttpHelper.InvalidContent();
            }
            #endregion

            #region Set Role Policy
            var status = await _rolePolicy.SetRolePolicy(data.RoleId, data.ResourceIds, UserId);

            return status;
            #endregion
        }
        catch (Exception ex)
        {
            return HttpHelper.ExceptionContent(ex);
        }
    }
    #endregion
}