using Microsoft.Extensions.AI;

namespace Dot.Models.Extensions
{
    public static class ChatRoleExtensions
    {
        public static (ChatRole, string) And(this ChatRole role, string roleString)
        {
            return (role, roleString);
        }

        public static bool AreEqual(this (ChatRole left, string right) role)
        {
            return role.left.Value.Equals(role.right);
        }
    }
}
