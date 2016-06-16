using codeConversion;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ProcessWordUnitTests
{
    
    
    /// <summary>
    ///This is a test class for ProcessWordTest and is intended
    ///to contain all ProcessWordTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProcessWordTest
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
        ///A test for retract
        ///</summary>
        [TestMethod()]
        public void retractTest()
        {
            string word = "retract";
            ProcessWord target = new ProcessWord(word);
            char expected = 'r';
            char actual = target.getNextChar();
            Assert.AreEqual(expected, actual);
            // read one more character
            actual = target.getNextChar();
            Assert.AreNotEqual(expected, actual);
            // return the character back
            target.retract(1);

          // get it back
            actual = target.getNextChar();
            Assert.AreEqual('e', actual);
        }

        /// <summary>
        ///A test for getNextChar
        ///</summary>
        [TestMethod()]
        public void getNextCharTest()
        {
            string word = "test";
            ProcessWord target = new ProcessWord(word); 
            char expected = 't'; 
            char actual;
            actual = target.getNextChar();
            Assert.AreEqual(expected, actual);
            
        }

        [TestMethod]
        public void getNextNonExistingCharacter()
        {
            string word = "t";
            ProcessWord target = new ProcessWord(word);
          
            // now get another character, it should throw an exception
            try {
                target.retract(1);
            } catch (ConversionException ex) {
                Assert.AreEqual("invalid character retraction", ex.Message);
            }
        }

        [TestMethod]
        public void retractNonExistingCharacter()
        {
            string word = "t";
            ProcessWord target = new ProcessWord(word);
            char expected = 't';
            char actual = target.getNextChar();
            Assert.AreEqual(expected, actual);
            // now get another character, it should throw an exception
            try {
                actual = target.getNextChar();

            } catch (ConversionException ex) {
                Assert.AreEqual("end of word enumeration", ex.Message);
            }
        }

        /// <summary>
        ///A test for discard
        ///</summary>
        [TestMethod()]
        public void discardTest()
        {
            string word = string.Empty; // TODO: Initialize to an appropriate value
            ProcessWord target = new ProcessWord(word); // TODO: Initialize to an appropriate value
            char expected = '\0'; // TODO: Initialize to an appropriate value
            char actual;
            actual = target.discard();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ProcessWord Constructor
        ///</summary>
        [TestMethod()]
        public void ProcessWordConstructorTest()
        {
            string word = string.Empty; // TODO: Initialize to an appropriate value
            ProcessWord target = new ProcessWord(word);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
