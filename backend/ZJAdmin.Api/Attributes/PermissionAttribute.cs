namespace ZJAdmin.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class PermissionAttribute : Attribute
{
    public string Code { get; }

    public PermissionAttribute(string code)
    {
        Code = code;
    }
}
