using StorageClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace StorageClientTest
{
    [TestClass]
    public class StorageClientWrapperTest
    {
        [TestMethod]
        public void StorageClientWrapper_GetBaseAddress()
        {
            StorageClientWrapper wrapper = new StorageClientWrapper();
            Guid instanceGuid = Guid.NewGuid();
            Guid dataGuid = Guid.NewGuid();

            wrapper.GetDocument(1, instanceGuid, dataGuid);
        }
    }
}
