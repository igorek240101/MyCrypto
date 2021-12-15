using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyCryptoAPI;
using MyCryptoAPI.Controllers;

namespace CryptTest
{
    [TestClass]
    public class CryptTest
    {

#if TEST
        [TestMethod]
        public void NotCryptTest()
        {
            Task<byte[]> task = CryptoController.Crypt("А", "Сообщение", CryptoController.Encrypt);
            task.Wait();
            Assert.AreEqual("Сообщение", Encoding.UTF8.GetString(task.Result));
        }

        [TestMethod]
        public void TwoThreCryptTest()
        {
            Task<byte[]> task = CryptoController.Crypt("ГД", "АБВ", CryptoController.Encrypt);
            task.Wait();
            Assert.AreEqual("ГЕЕ", Encoding.UTF8.GetString(task.Result));
        }

        [TestMethod]
        public void TwoFourCryptTest()
        {
            Task<byte[]> task = CryptoController.Crypt("ГД", "АБВГ", CryptoController.Encrypt);
            task.Wait();
            Assert.AreEqual("ГЕЕЖ", Encoding.UTF8.GetString(task.Result));
        }

        [TestMethod]
        public void ThreThreCryptTest()
        {
            Task<byte[]> task = CryptoController.Crypt("ГДЕ", "АБВ", CryptoController.Encrypt);
            task.Wait();
            Assert.AreEqual("ГЕЖ", Encoding.UTF8.GetString(task.Result));
        }

        [TestMethod]
        public void FourThreCryptTest()
        {
            Task<byte[]> task = CryptoController.Crypt("ГДЕЁ", "АБВ", CryptoController.Encrypt);
            task.Wait();
            Assert.AreEqual("ГЕЖ", Encoding.UTF8.GetString(task.Result));
        }
#endif
    }
}
