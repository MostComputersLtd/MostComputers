namespace MOSTComputers.UI.Web.Models.Authentication;

public sealed class UserRoles : IEquatable<UserRoles>
{
    internal const string AdminRoleName = "Admin";
    internal const string XmlRelationEditorRoleName = "XmlRelationEditor";
    internal const string ProductEditorRoleName = "ProductEditor";
    internal const string EmployeeRoleName = "Employee";
    internal const string UserRoleName = "User";

    private UserRoles(string roleName, int value)
    {
        _roleName = roleName;
        _value = value;
    }

    private readonly string _roleName;
    private readonly int _value;

    public string RoleName => _roleName;
    public int Value => _value;

    public static readonly UserRoles Admin = new(AdminRoleName, 0);
    public static readonly UserRoles XmlRelationEditor = new(XmlRelationEditorRoleName, 1);
    public static readonly UserRoles ProductEditor = new(ProductEditorRoleName, 2);
    public static readonly UserRoles Employee = new(EmployeeRoleName, 3);
    public static readonly UserRoles User = new(UserRoleName, 4);

    public static UserRoles? GetRoleWithValue(int value)
    {
        return value switch
        {
            0 => Admin,
            1 => XmlRelationEditor,
            2 => ProductEditor,
            3 => Employee,
            4 => User,
            _ => null
        };
    }

    public static UserRoles? GetRoleWithName(string name)
    {
        return name switch
        {
            AdminRoleName => Admin,
            XmlRelationEditorRoleName => XmlRelationEditor,
            ProductEditorRoleName => ProductEditor,
            EmployeeRoleName => Employee,
            UserRoleName => User,
            _ => null
        };
    }

    public bool Equals(UserRoles? other)
    {
        if (other is null) return false;

        return _value == other._value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as UserRoles);
    }

    public override int GetHashCode()
    {
        return _value;
    }
}