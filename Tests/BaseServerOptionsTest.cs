using HldsLauncher.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Linq;

namespace Tests
{
    
    
    /// <summary>
    ///This is a test class for BaseServerOptionsTest and is intended
    ///to contain all BaseServerOptionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BaseServerOptionsTest
    {
        private TestContext testContextInstance;

        private XNode xServerForTest = XElement.Parse(@"<Server>
          <Game>cstrike</Game>
          <ConsoleType>Integrated</ConsoleType>
          <Map></Map>
          <MaxPlayers>12</MaxPlayers>
          <RconPassword>turboPass</RconPassword>
          <Vac>True</Vac>
          <NoIpx>False</NoIpx>
          <NoMaster>False</NoMaster>
          <SvLan>False</SvLan>
          <ConsolePositionX>30</ConsolePositionX>
          <ConsolePositionY>30</ConsolePositionY>
          <Ip>0.0.0.0</Ip>
          <Port></Port>
          <HostName>and1gaming.org.ua</HostName>
          <ExecutablePath></ExecutablePath>
          <Priority>Normal</Priority>
          <ProcessorAffinity>0</ProcessorAffinity>
          <AutoRestart>True</AutoRestart>
          <ActiveServer>True</ActiveServer>
          <AdditionalCommandLineArgs></AdditionalCommandLineArgs>
        </Server>");

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        internal virtual BaseServerOptions CreateBaseServerOptions()
        {
            // TODO: Instantiate an appropriate concrete class.
            BaseServerOptions target = new GoldSourceServerOptions();
            return target;
        }

        /// <summary>
        ///A test for SaveToXml
        ///</summary>
        [TestMethod()]
        public void GetXmlTest()
        {
            BaseServerOptions target = CreateBaseServerOptions(); // TODO: Initialize to an appropriate value
            XNode expected = xServerForTest; // TODO: Initialize to an appropriate value
            XNode actual;
            actual = target.GetXml();
            //if (expected.Value != actual.Value)
            //{
            //    Assert.Fail();
            //}
            Assert.AreEqual(expected.ToString(), actual.ToString());
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateFromXml
        ///</summary>
        [TestMethod()]
        public void CreateFromXmlTest()
        {
            XNode xServer = xServerForTest; // TODO: Initialize to an appropriate value
            BaseServerOptions expected = new GoldSourceServerOptions(); // TODO: Initialize to an appropriate value
            BaseServerOptions actual;
            actual = BaseServerOptions.CreateFromXml<GoldSourceServerOptions>(xServer);
            Assert.AreEqual(expected.CommandLine(), actual.CommandLine());
            //Assert.Inconclusive("No appropriate type parameter is found to satisfies the type constraint(s) of T. " +
            //        "Please call CreateFromXmlTestHelper<T>() with appropriate type parameters.");
        }
    }
}
