using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Messenger.Models.Database;
using Messenger.ViewModels;

namespace PegeonUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async void GetUserID_test()
        {
            var bd = new SQLiteDb();
            var ID = bd.GetMyID();
            if(await (new LoginVM().CheckIfLoggedIn()))
                Assert.IsNotNull(ID);
            else
                Assert.IsNull(ID);
        }

        [TestMethod]
        public async void GetAccessToken_test()
        {
            var bd = new SQLiteDb();
            var token = bd.GetAccessTokenAsync();

            if (await (new LoginVM().CheckIfLoggedIn()))
                Assert.IsNotNull(token);
            else
                Assert.IsNull(token);
        }

        [TestMethod]
        public async void GetRefreshToken_test()
        {
            var bd = new SQLiteDb();
            var token = bd.GetRefreshTokenAsync();

            if (await (new LoginVM().CheckIfLoggedIn()))
                Assert.IsNotNull(token);
            else
                Assert.IsNull(token);
        }
    }
}
