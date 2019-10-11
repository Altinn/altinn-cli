using StorageClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AltinnCLI.Services.Storage;

namespace StorageClientTest
{
    [TestClass]
    public class StorageClientTest
    {
        [TestMethod]
        public void StorageClientWrapper_GetBaseAddress()
        {
            StorageClientWrapper wrapper = new StorageClientWrapper();
            wrapper.BaseAddress = "BaseAddress";
            Guid instanceGuid = Guid.NewGuid();
            Guid dataGuid = Guid.NewGuid();

            wrapper.GetDocument(1, instanceGuid, dataGuid);
        }
    }
}
