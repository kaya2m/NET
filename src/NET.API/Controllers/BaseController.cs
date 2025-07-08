using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET.Application.Common.Interfaces;
using NET.Application.Common.Models;
using System;

namespace NET.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ICurrentUserService CurrentUserService;

        protected BaseController(ICurrentUserService currentUserService)
        {
            CurrentUserService = currentUserService;
        }

        protected IActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(new ApiResponse<T>
                {
                    Success = true,
                    Data = result.Data,
                    Message = result.Message
                });
            }

            return BadRequest(new ApiResponse<T>
            {
                Success = false,
                Errors = result.Errors
            });
        }

        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Message = result.Message
                });
            }

            return BadRequest(new
            {
                Success = false,
                Errors = result.Errors
            });
        }

        protected IActionResult HandlePagedResult<T>(Result<PagedResult<T>> result)
        {
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    Success = true,
                    Data = result.Data.Data,
                    PageNumber = result.Data.PageNumber,
                    PageSize = result.Data.PageSize,
                    TotalCount = result.Data.TotalCount,
                    TotalPages = result.Data.TotalPages,
                    HasPreviousPage = result.Data.HasPreviousPage,
                    HasNextPage = result.Data.HasNextPage
                });
            }

            return BadRequest(new
            {
                Success = false,
                Errors = result.Errors
            });
        }

        protected Guid GetCurrentUserId()
        {
            return CurrentUserService.UserId ?? throw new UnauthorizedAccessException("User not authenticated");
        }

        protected Guid GetCurrentTenantId()
        {
            return CurrentUserService.TenantId ?? throw new UnauthorizedAccessException("Tenant not available");
        }

        protected string GetCurrentUserName()
        {
            return CurrentUserService.UserName ?? "Unknown";
        }

        protected bool IsInRole(string role)
        {
            return User.IsInRole(role);
        }
    }
}