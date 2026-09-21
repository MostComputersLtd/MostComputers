namespace MOSTComputers.Services.Authentication.Models;

public enum ApiSecretPermissions
{
    ReadInvoices = 0,
    ReadWarrantyCards = 1,
    ReadOrders = 2,
}

// public sealed class ApiSecretPermissions : IEquatable<ApiSecretPermissions>
// {
//     private ApiSecretPermissions(int value)
//     {
//         _value = value;
//     }
// 
//     private readonly int _value;
// 
//     public static readonly ApiSecretPermissions ReadInvoices = new(0);
//     public static readonly ApiSecretPermissions ReadWarrantyCards = new(1);
//     public static readonly ApiSecretPermissions ReadOrders = new(2);
// 
//     public bool Equals(ApiSecretPermissions? other)
//     {
//         if (other is null) return false;
// 
//         return _value == other._value;
//     }
// 
//     public override bool Equals(object? obj)
//     {
//         return Equals(obj as ApiSecretPermissions);
//     }
// 
//     public static bool operator ==(ApiSecretPermissions? left, ApiSecretPermissions? right)
//     {
//         if (left is null) return right is null;
// 
//         return left.Equals(right);
//     }
// 
//     public static bool operator !=(ApiSecretPermissions? left, ApiSecretPermissions? right)
//     {
//         return !(left == right);
//     }
// 
//     public override int GetHashCode()
//     {
//         return _value.GetHashCode();
//     }
// }
