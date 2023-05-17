using Messenger.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace PegeonUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async void GetUserNicknameWithCorrectID()
        {
            var vm = new MessengerVM();
            try
            {
                var username = await vm.GetUserNickname(1);
                if (String.IsNullOrEmpty(username))
                    Assert.Fail("username был пустым");
                Assert.IsTrue(true);
            }
            catch(Exception ex) 
            {
                Assert.Fail("Исключение err: " + ex.Message);
            }
        }
    }
}
