using Dot.Models.Extensions;
using Microsoft.Extensions.AI;

namespace Dot.Models.Tests
{
    public class ChatRoleExtensionsTests
    {
        public static IEnumerable<object[]> GetChatRoleData()
        {
            yield return new object[] { ChatRole.Assistant, "assistant", true };
            yield return new object[] { ChatRole.User, "user", true };
            yield return new object[] { ChatRole.System, "system", true };
            yield return new object[] { ChatRole.Tool, "tool", true };
            yield return new object[] { ChatRole.Tool, "non-matching", false };
        }

        [Theory]
        [MemberData(nameof(GetChatRoleData))]
        public void AreEqual_ReturnsTrueForMatcingPairs_ReturnsFalseForNonMatchingPairs(ChatRole role, string roleString, bool expectedResult)
        {
            Assert.Equal(expectedResult, role.And(roleString).AreEqual());
        }
    }
}
