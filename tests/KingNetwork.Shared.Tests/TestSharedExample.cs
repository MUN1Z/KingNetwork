using Xunit;
using XUnitPriorityOrderer;

namespace KingNetwork.Shared.Tests
{
    [Order(3)]
	public class SharedTests
	{
		#region constructors

		public SharedTests()
		{

		}

		#endregion

		#region tests implementations

		[Fact, Order(1)]
		public void Test()
		{
			Assert.True(true);
		}

        #endregion
    }
}
