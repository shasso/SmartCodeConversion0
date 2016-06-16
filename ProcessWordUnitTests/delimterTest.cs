using codeConversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ProcessWordUnitTests
{
    
    
    /// <summary>
    ///This is a test class for delimterTest and is intended
    ///to contain all delimterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class delimterTest
    {


        private TestContext testContextInstance;

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


        /// <summary>
        ///A test for END
        ///</summary>
        [TestMethod()]
        public void ENDTest()
        {
            char begin = '<'; 
            char end = '>'; 
            Delimiter target = new Delimiter(begin, end); 
            char expected = '>'; 
            char actual;
         
            actual = target.END;
            Assert.AreEqual(expected, actual);
           
        }

        /// <summary>
        ///A test for START
        ///</summary>
        [TestMethod()]
        public void BEGINTest()
        {
            char begin = '<';
            char end = '>'; 
            Delimiter target = new Delimiter(begin, end); 
            char expected = '<'; 
            char actual;
            
            actual = target.START;
            Assert.AreEqual(expected, actual);
         
        }

        /// <summary>
        ///A test for Delimiter Constructor
        ///</summary>
        [TestMethod()]
        public void delimterConstructorTest()
        {
            char begin = '\0'; // TODO: Initialize to an appropriate value
            char end = '\0'; // TODO: Initialize to an appropriate value
            Delimiter target = new Delimiter(begin, end);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
