using System.ComponentModel.DataAnnotations;

namespace NCDC.WebBootstrapper.Controllers.AuthUser.Update;

public class AuthUserUpdateRequest
{
    [Display(Name = "用户id"),Required(ErrorMessage = "{0}不能为空")]
    public string Id { get; set; } = null!;
    /// <summary>
    /// 密码
    /// </summary>
    [Display(Name = "密码"),Required(ErrorMessage = "{0}不能为空")]
    public string Password { get; set; } = null!;
    /// <summary>
    /// 限制地址
    /// </summary>
    [Display(Name = "限制地址"),Required(ErrorMessage = "{0}不能为空")]
    public string HostName { get; set; } = null!;
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnable { get; set; }
}