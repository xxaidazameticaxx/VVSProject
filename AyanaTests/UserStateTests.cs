using Ayana.Paterni;

namespace AyanaTests
{
    [TestClass]
    public class UserStateTests
    {
        // written by : Aida Zametica
        [TestMethod]
        public void UserState_Initialization_ReturnsNotNullInstance()
        {
            UserState userState = new UserState();

            Assert.IsNotNull(userState);
        }

        // written by : Aida Zametica
        [TestMethod]
        public void CanPurchase_WhenInitialState_ReturnsFalse()
        {
            UserState userState = new UserState();

            bool canPurchase = userState.CanPurchase();

            Assert.IsFalse(canPurchase); // Assuming the initial state is GuestState that can not buy products
        }

        // written by : Aida Zametica
        [TestMethod]
        public void CanView_WhenInitialState_ReturnsTrue()
        {
            UserState userState = new UserState();

            bool canView = userState.CanView();

            Assert.IsTrue(canView); // Assuming the initial state is GuestState that can view the products
        }

        // written by : Aida Zametica
        [TestMethod]
        public void SetState_WithGuestState_SetsCorrectStateProperties()
        {
            UserState userState = new UserState();

            userState.SetState(new GuestState());

            Assert.IsFalse(userState.CanPurchase());
            Assert.IsTrue(userState.CanView());
        }

    }
}
