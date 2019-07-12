using Liquibook.NET.Book;
using Liquibook.NET.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Test
{
    [TestClass]
    public class DepthTests
    {
        private static bool VerifyLevel(DepthLevel level, Price price, int orderCount, Quantity aggregateQuantity)
        {
            var matched = true;
            if (level.Price != price) matched = false;
            if (level.OrderCount != orderCount) matched = false;
            if (level.AggregateQty != aggregateQuantity) matched = false;
            return matched;
        }

        [TestMethod]
        public void TestAddBid()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1234, 100, true);
            var firstBid = depth.Bids.First().Value;
            Assert.IsTrue(VerifyLevel(firstBid, 1234, 1, 100));
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            Assert.AreEqual(1, depth.Bids.Count);
        }

        [TestMethod]
        public void TestAddBids()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1234, 100, true);
            depth.AddOrder(1234, 200, true);
            depth.AddOrder(1234, 300, true);
            var firstBid = depth.Bids.First().Value;
            Assert.IsTrue(VerifyLevel(firstBid, 1234, 3, 600));
            Assert.IsTrue(cc.VerifyBidChanged(true,false, false, false, false));
            Assert.AreEqual(1, depth.Bids.Count);
        }

        [TestMethod]
        public void TestAppendBidLevels()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1236, 300, true);
            depth.AddOrder(1235, 200, true);
            depth.AddOrder(1232, 100, true);
            depth.AddOrder(1235, 400, true);

            var bid0 = depth.Bids.ElementAt(0).Value;
            var bid1 = depth.Bids.ElementAt(1).Value;
            var bid2 = depth.Bids.ElementAt(2).Value;
            Assert.IsTrue(VerifyLevel(bid0, 1236, 1, 300));
            Assert.IsTrue(VerifyLevel(bid1, 1235, 2, 600));
            Assert.IsTrue(VerifyLevel(bid2, 1232, 1, 100));
            Assert.IsTrue(cc.VerifyBidChanged(true, true, true, false, false));
            Assert.AreEqual(3, depth.Bids.Count);
        }

        [TestMethod]
        public void TestInsertBidLevels()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1234, 800, true);

            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false)); 
            cc.Reset();

            depth.AddOrder(1232, 100, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1236, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, true, true, false, false)); 
            cc.Reset();

            depth.AddOrder(1235, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, true, true, false));
            cc.Reset();

            depth.AddOrder(1234, 900, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1231, 700, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, false, true));
            cc.Reset();

            depth.AddOrder(1235, 400, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1231, 500, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, false, true));
            cc.Reset();

            depth.AddOrder(1233, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, true, true));
            cc.Reset();

            var bid0 = depth.Bids.ElementAt(0).Value;
            var bid1 = depth.Bids.ElementAt(1).Value;
            var bid2 = depth.Bids.ElementAt(2).Value;
            var bid3 = depth.Bids.ElementAt(3).Value;
            var bid4 = depth.Bids.ElementAt(4).Value;
            Assert.IsTrue(VerifyLevel(bid0, 1236, 1,  300));
            Assert.IsTrue(VerifyLevel(bid1, 1235, 2,  600));
            Assert.IsTrue(VerifyLevel(bid2, 1234, 2, 1700));
            Assert.IsTrue(VerifyLevel(bid3, 1233, 1,  200));
            Assert.IsTrue(VerifyLevel(bid4, 1232, 1,  100));
            Assert.AreEqual(5, depth.Bids.Count);
        }

        [TestMethod]
        public void TestInsertBidLevelsPast5()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1234, 800, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1236, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, true, true, false, false));
            cc.Reset();

            depth.AddOrder(1231, 700, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, true, false));
            cc.Reset();

            depth.AddOrder(1234, 900, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, true, true, true));
            cc.Reset();

            depth.AddOrder(1235, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1231, 500, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, false, true));
            cc.Reset();

            depth.AddOrder(1230, 200, true);

            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, false, false));
            cc.Reset();
            depth.AddOrder(1229, 200, true);

            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, false, false));
            cc.Reset();

            var bid0 = depth.Bids.ElementAt(0).Value;
            var bid1 = depth.Bids.ElementAt(1).Value;
            var bid2 = depth.Bids.ElementAt(2).Value;
            var bid3 = depth.Bids.ElementAt(3).Value;
            var bid4 = depth.Bids.ElementAt(4).Value;
            
            Assert.IsTrue(VerifyLevel(bid0, 1236, 1,  300));
            Assert.IsTrue(VerifyLevel(bid1, 1235, 2,  600));
            Assert.IsTrue(VerifyLevel(bid2, 1234, 2, 1700));
            Assert.IsTrue(VerifyLevel(bid3, 1232, 1,  100));
            Assert.IsTrue(VerifyLevel(bid4, 1231, 2, 1200));
            Assert.AreEqual(5, depth.Bids.Count);
        }

        [TestMethod]
        public void TestInsertBidLevelsTruncate5()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1234, 800, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1236, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, true, true, false, false));
            cc.Reset();

            depth.AddOrder(1231, 700, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, true, false));
            cc.Reset();

            depth.AddOrder(1234, 900, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, true, true, true));
            cc.Reset();

            depth.AddOrder(1235, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1231, 500, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, false, true));
            cc.Reset();

            depth.AddOrder(1230, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1238, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, true, true, true, true));
            cc.Reset();

            depth.AddOrder(1238, 250, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1237, 500, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, true, true, true));
            cc.Reset();

            var bid0 = depth.Bids.ElementAt(0).Value;
            var bid1 = depth.Bids.ElementAt(1).Value;
            var bid2 = depth.Bids.ElementAt(2).Value;
            var bid3 = depth.Bids.ElementAt(3).Value;
            var bid4 = depth.Bids.ElementAt(4).Value;
            Assert.IsTrue(VerifyLevel(bid0, 1238, 2,  450));
            Assert.IsTrue(VerifyLevel(bid1, 1237, 1,  500));
            Assert.IsTrue(VerifyLevel(bid2, 1236, 1,  300));
            Assert.IsTrue(VerifyLevel(bid3, 1235, 2,  600));
            Assert.IsTrue(VerifyLevel(bid4, 1234, 2, 1700));
            Assert.AreEqual(5, depth.Bids.Count);
        }
        
        [TestMethod]
        public void TestCloseBid()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1234, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1234, 500, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            Assert.IsTrue(!depth.CloseOrder(1234, 300, true)); // Does not erase
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));

            var firstBid = depth.Bids.First().Value;
            Assert.IsTrue(VerifyLevel(firstBid, 1234, 1, 500));
            Assert.AreEqual(1, depth.Bids.Count);
        }

        [TestMethod]
        public void TestCloseEraseBid()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1235, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1234, 500, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1233, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, true, false, false));
            cc.Reset();

            Assert.IsTrue(!depth.CloseOrder(1235, 300, true)); // Does not erase
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            Assert.IsTrue(depth.CloseOrder(1235, 400, true)); // Erase
            Assert.IsTrue(cc.VerifyBidChanged(true, true, true, false, false));

            var bid0 = depth.Bids.ElementAt(0).Value;
            var bid1 = depth.Bids.ElementAt(1).Value;
            Assert.IsTrue(VerifyLevel(bid0, 1234, 1, 500));
            Assert.IsTrue(VerifyLevel(bid1, 1233, 1, 200));
            Assert.AreEqual(2, depth.Bids.Count);
        }

        [TestMethod]
        public void TestAddCloseAddBid()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1234, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();
  
            depth.CloseOrder(1234, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();
  
            depth.AddOrder(1233, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();
            
            var bid0 = depth.Bids.ElementAt(0).Value;
            Assert.IsTrue(VerifyLevel(bid0, 1233, 1, 200));
            Assert.AreEqual(1, depth.Bids.Count);
        }

        [TestMethod]
        public void TestAddCloseAddHigherBid()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1234, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.CloseOrder(1234, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            var bid = depth.Bids.ElementAt(0).Value;
            Assert.IsTrue(VerifyLevel(bid, 1235, 1, 200));
            Assert.AreEqual(1, depth.Bids.Count);
        }

        [TestMethod]
        public void TestCloseBidsFreeLevels()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);

            depth.AddOrder(1234, 800, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1236, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, true, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, true, true, false));
            cc.Reset();

            depth.AddOrder(1234, 900, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1231, 700, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, false, true));
            cc.Reset();

            depth.AddOrder(1235, 400, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1231, 500, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, false, true));
            cc.Reset();

            depth.CloseOrder(1234, 900, true); // No erase
            Assert.IsTrue(cc.VerifyBidChanged(false, false, true, false, false));
            cc.Reset();

            depth.CloseOrder(1232, 100, true); // Erase
            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, true, true));
            cc.Reset();

            depth.CloseOrder(1236, 300, true); // Erase
            Assert.IsTrue(cc.VerifyBidChanged(true, true, true, true, false));
            cc.Reset();

            var bid0 = depth.Bids.ElementAt(0).Value;
            var bid1 = depth.Bids.ElementAt(1).Value;
            var bid2 = depth.Bids.ElementAt(2).Value;
            Assert.IsTrue(VerifyLevel(bid0, 1235, 2, 600));
            Assert.IsTrue(VerifyLevel(bid1, 1234, 1, 800));
            Assert.IsTrue(VerifyLevel(bid2, 1231, 2, 1200));
            Assert.AreEqual(3, depth.Bids.Count);

            depth.AddOrder(1233, 350, true); // Insert
            Assert.IsTrue(cc.VerifyBidChanged(false, false, true, true, false));
            cc.Reset();

            depth.AddOrder(1236, 300, true); // Insert
            Assert.IsTrue(cc.VerifyBidChanged(true, true, true, true, true));
            cc.Reset();

            depth.AddOrder(1231, 700, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, false, false, true));
            cc.Reset();
            
            bid0 = depth.Bids.ElementAt(0).Value;
            bid1 = depth.Bids.ElementAt(1).Value;
            bid2 = depth.Bids.ElementAt(2).Value;
            var bid3 = depth.Bids.ElementAt(3).Value;
            var bid4 = depth.Bids.ElementAt(4).Value;
            Assert.IsTrue(VerifyLevel(bid0, 1236, 1, 300));
            Assert.IsTrue(VerifyLevel(bid1, 1235, 2, 600));
            Assert.IsTrue(VerifyLevel(bid2, 1234, 1, 800));
            Assert.IsTrue(VerifyLevel(bid3, 1233, 1, 350));
            Assert.IsTrue(VerifyLevel(bid4, 1231, 3, 1900));
            Assert.AreEqual(5, depth.Bids.Count);
        }

        [TestMethod]
        public void TestIncreaseBid()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1236, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1232, 37, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, true, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1232, 41, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, true, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1235, 201, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            var bid0 = depth.Bids.ElementAt(0).Value;
            var bid1 = depth.Bids.ElementAt(1).Value;
            var bid2 = depth.Bids.ElementAt(2).Value;
            Assert.IsTrue(VerifyLevel(bid0, 1236, 1, 300));
            Assert.IsTrue(VerifyLevel(bid1, 1235, 2, 801));
            Assert.IsTrue(VerifyLevel(bid2, 1232, 1, 178));
            Assert.AreEqual(3, depth.Bids.Count);
        }

        [TestMethod]
        public void TestDecreaseBid()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1236, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1236, -37, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1236, -41, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1235, -201, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            var bid0 = depth.Bids.ElementAt(0).Value;
            var bid1 = depth.Bids.ElementAt(1).Value;
            var bid2 = depth.Bids.ElementAt(2).Value;
            Assert.IsTrue(VerifyLevel(bid0, 1236, 1, 222));
            Assert.IsTrue(VerifyLevel(bid1, 1235, 2, 399));
            Assert.IsTrue(VerifyLevel(bid2, 1232, 1, 100));
            Assert.AreEqual(3, depth.Bids.Count);
        }

        [TestMethod]
        public void TestIncreaseDecreaseBid()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1236, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1236, 37, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1235, -41, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1232, 60, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, true, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1236, -41, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1236, 210, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();
            
            var bid0 = depth.Bids.ElementAt(0).Value;
            var bid1 = depth.Bids.ElementAt(1).Value;
            var bid2 = depth.Bids.ElementAt(2).Value;
            Assert.IsTrue(VerifyLevel(bid0, 1236, 1, 506));
            Assert.IsTrue(VerifyLevel(bid1, 1235, 2, 559));
            Assert.IsTrue(VerifyLevel(bid2, 1232, 1, 160));
            Assert.AreEqual(3, depth.Bids.Count);
        }
        
        [TestMethod]
        public void TestAddAsk()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1234, 100, false);
            var firstAsk = depth.Asks.First().Value;
            Assert.IsTrue(VerifyLevel(firstAsk, 1234, 1, 100));
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            Assert.AreEqual(1, depth.Asks.Count);
        }

        [TestMethod]
        public void TestAddAsks()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1234, 100, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1234, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1234, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));

            var firstAsk = depth.Asks.First().Value;
            Assert.IsTrue(VerifyLevel(firstAsk, 1234, 3, 600));
            Assert.AreEqual(1, depth.Asks.Count);
        }

        [TestMethod]
        public void TestAddCloseAddAsk()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1234, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.CloseOrder(1234, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1233, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();
            var ask0 = depth.Asks.ElementAt(0).Value;
            Assert.IsTrue(VerifyLevel(ask0, 1233, 1, 200));
            Assert.AreEqual(1, depth.Asks.Count);
        }

        [TestMethod]
        public void TestAddCloseAddHigherAsk()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1234, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.CloseOrder(1234, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();
            var ask0 = depth.Asks.ElementAt(0).Value;
            Assert.IsTrue(VerifyLevel(ask0, 1235, 1, 200));
            Assert.AreEqual(1, depth.Asks.Count);
        }

        [TestMethod]
        public void TestAppendAskLevels()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1236, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, true, false, false, false));
            cc.Reset();

            var ask1 = depth.Asks.First().Value;
            var ask2 = depth.Asks.ElementAt(1).Value;
            var ask3 = depth.Asks.ElementAt(2).Value;

            Assert.IsTrue(VerifyLevel(ask1, 1232, 1, 100));
            Assert.IsTrue(VerifyLevel(ask2, 1235, 2, 600));
            Assert.IsTrue(VerifyLevel(ask3, 1236, 1, 300));
            Assert.AreEqual(3, depth.Asks.Count);
        }

        [TestMethod]
        public void TestCloseAsk()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1234, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1234, 500, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();
            Assert.IsFalse(depth.CloseOrder(1234, 300, false)); // Does not erase
            var firstAsk = depth.Asks.First().Value;
            Assert.IsTrue(VerifyLevel(firstAsk, 1234, 1, 500));
            Assert.AreEqual(1, depth.Asks.Count);
        }

        [TestMethod]
        public void TestCloseAsksFreeLevels()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);

            depth.AddOrder(1234, 800, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1236, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, true, false));
            cc.Reset();

            depth.AddOrder(1234, 900, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1231, 700, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, true, true, true));
            cc.Reset();

            depth.AddOrder(1235, 400, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, false, true, false));
            cc.Reset();

            depth.AddOrder(1231, 500, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.CloseOrder(1234, 900, false); // does not erase
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();

            depth.CloseOrder(1232, 100, false); // erase
            Assert.IsTrue(cc.VerifyAskChanged(false, true, true, true, true));
            cc.Reset();
            depth.CloseOrder(1236, 100, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, false, true, false));
            cc.Reset();
            
            var ask0 = depth.Asks.First().Value;
            var ask1 = depth.Asks.ElementAt(1).Value;
            var ask2 = depth.Asks.ElementAt(2).Value;
            
            Assert.IsTrue(VerifyLevel(ask0, 1231, 2, 1200));
            Assert.IsTrue(VerifyLevel(ask1, 1234, 1, 800));
            Assert.IsTrue(VerifyLevel(ask2, 1235, 2, 600));
            
            Assert.AreEqual(3, depth.Asks.Count);
            depth.AddOrder(1233, 350, false);
            depth.AddOrder(1236, 300, false);
            depth.AddOrder(1231, 700, false);
            
            ask0 = depth.Asks.First().Value;
            ask1 = depth.Asks.ElementAt(1).Value;
            ask2 = depth.Asks.ElementAt(2).Value;
            var ask3 = depth.Asks.ElementAt(3).Value;
            var ask4 = depth.Asks.ElementAt(4).Value;
            Assert.IsTrue(VerifyLevel(ask0, 1231, 3, 1900));
            Assert.IsTrue(VerifyLevel(ask1, 1233, 1, 350));
            Assert.IsTrue(VerifyLevel(ask2, 1234, 1, 800));
            Assert.IsTrue(VerifyLevel(ask3, 1235, 2, 600));
            Assert.IsTrue(VerifyLevel(ask4, 1236, 1, 300));
            Assert.AreEqual(5, depth.Asks.Count);
        }

        [TestMethod]
        public void TestCloseEraseAsk()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1233, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1234, 500, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1233, 400, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            Assert.IsFalse(depth.CloseOrder(1233, 300, false)); // Does not erase
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            Assert.IsTrue(depth.CloseOrder(1233, 400, false)); // Erase
            Assert.IsTrue(cc.VerifyAskChanged(true, true, false, false, false));
            cc.Reset();
            var firstAsk = depth.Asks.First().Value;
            Assert.IsTrue(VerifyLevel(firstAsk, 1234, 1, 500));
            Assert.AreEqual(1, depth.Asks.Count);
        }

        [TestMethod]
        public void TestInsertAskLevels()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1234, 800, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));

            cc.Reset();

            depth.AddOrder(1232, 100, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1236, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, true, false));
            cc.Reset();

            depth.AddOrder(1234, 900, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, true, false, false, false));
            cc.Reset();
            depth.AddOrder(1231, 700, false);
            depth.AddOrder(1235, 400, false);
            depth.AddOrder(1231, 500, false);
            var ask0 = depth.Asks.ElementAt(0).Value;
            var ask1 = depth.Asks.ElementAt(1).Value;
            var ask2 = depth.Asks.ElementAt(2).Value;
            var ask3 = depth.Asks.ElementAt(3).Value;
            var ask4 = depth.Asks.ElementAt(4).Value;
            Assert.IsTrue(VerifyLevel(ask0, 1231, 2, 1200));
            Assert.IsTrue(VerifyLevel(ask1, 1232, 1, 100));
            Assert.IsTrue(VerifyLevel(ask2, 1234, 2, 1700));
            Assert.IsTrue(VerifyLevel(ask3, 1235, 2, 600));
            Assert.IsTrue(VerifyLevel(ask4, 1236, 1, 300));
            Assert.AreEqual(5, depth.Asks.Count);
        }

        [TestMethod]
        public void TestInsertAskLevelsPast5()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1234, 800, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1236, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1231, 700, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, true, true, false));
            cc.Reset();

            depth.AddOrder(1234, 900, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, false, true, true));
            cc.Reset();

            depth.AddOrder(1235, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, false, true, false));
            cc.Reset();

            depth.AddOrder(1231, 500, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1230, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, true, true, true));
            cc.Reset();

            depth.AddOrder(1229, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, true, true, true));
            cc.Reset();

            var ask0 = depth.Asks.ElementAt(0).Value;
            var ask1 = depth.Asks.ElementAt(1).Value;
            var ask2 = depth.Asks.ElementAt(2).Value;
            var ask3 = depth.Asks.ElementAt(3).Value;
            var ask4 = depth.Asks.ElementAt(4).Value;
            Assert.IsTrue(VerifyLevel(ask0, 1229, 1, 200));
            Assert.IsTrue(VerifyLevel(ask1, 1230, 1, 200));
            Assert.IsTrue(VerifyLevel(ask2, 1231, 2, 1200));
            Assert.IsTrue(VerifyLevel(ask3, 1232, 1, 100));
            Assert.IsTrue(VerifyLevel(ask4, 1234, 2, 1700));
            Assert.AreEqual(5, depth.Asks.Count);
        }

        [TestMethod]
        public void TestInsertAskLevelsTruncate5()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            depth.AddOrder(1234, 800, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1236, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1231, 700, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, true, true, false));
            cc.Reset();

            depth.AddOrder(1234, 900, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, false, true, true));
            cc.Reset();

            depth.AddOrder(1235, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, false, true, false));
            cc.Reset();

            depth.AddOrder(1231, 500, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1230, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, true, true, true));
            cc.Reset();

            depth.AddOrder(1238, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 250, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1237, 500, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, false, false, false));
            cc.Reset();

            var ask0 = depth.Asks.ElementAt(0).Value;
            var ask1 = depth.Asks.ElementAt(1).Value;
            var ask2 = depth.Asks.ElementAt(2).Value;
            var ask3 = depth.Asks.ElementAt(3).Value;
            var ask4 = depth.Asks.ElementAt(4).Value;
            Assert.IsTrue(VerifyLevel(ask0, 1230, 1, 200));
            Assert.IsTrue(VerifyLevel(ask1, 1231, 2, 1200));
            Assert.IsTrue(VerifyLevel(ask2, 1232, 2, 350));
            Assert.IsTrue(VerifyLevel(ask3, 1234, 2, 1700));
            Assert.IsTrue(VerifyLevel(ask4, 1235, 2, 600));
            Assert.AreEqual(5, depth.Asks.Count);
        }

        [TestMethod]
        public void TestIncreaseAsk()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1236, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, true, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1232, 37, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1232, 41, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1235, 201, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, true, false, false, false));
            cc.Reset();

            var ask0 = depth.Asks.First().Value;
            var ask1 = depth.Asks.ElementAt(1).Value;
            var ask2 = depth.Asks.ElementAt(2).Value;
            
            Assert.IsTrue(VerifyLevel(ask0, 1232, 1, 178));
            Assert.IsTrue(VerifyLevel(ask1, 1235, 2, 801));
            Assert.IsTrue(VerifyLevel(ask2, 1236, 1, 300));
            Assert.AreEqual(3, depth.Asks.Count);
        }

        [TestMethod]
        public void TestDecreaseAsk()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1236, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, true, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1236, -37, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1236, -41, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1235, -201, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, true, false, false, false));
            cc.Reset();
            
            var ask0 = depth.Asks.First().Value;
            var ask1 = depth.Asks.ElementAt(1).Value;
            var ask2 = depth.Asks.ElementAt(2).Value;
            Assert.IsTrue(VerifyLevel(ask0, 1232, 1, 100));
            Assert.IsTrue(VerifyLevel(ask1, 1235, 2, 399));
            Assert.IsTrue(VerifyLevel(ask2, 1236, 1, 222));
            Assert.AreEqual(3, depth.Asks.Count);
        }

        [TestMethod]
        public void TestIncreaseDecreaseAsk()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1236, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, true, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1236, 37, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1235, -41, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, true, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1232, 51, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1236, -41, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();

            depth.ChangeOrderQuantity(1236, 201, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, false, true, false, false));
            cc.Reset();
            
            var ask0 = depth.Asks.First().Value;
            var ask1 = depth.Asks.ElementAt(1).Value;
            var ask2 = depth.Asks.ElementAt(2).Value;
            Assert.IsTrue(VerifyLevel(ask0, 1232, 1, 151));
            Assert.IsTrue(VerifyLevel(ask1, 1235, 2, 559));
            Assert.IsTrue(VerifyLevel(ask2, 1236, 1, 497));
            Assert.AreEqual(3, depth.Asks.Count);
        }

        [TestMethod]
        public void TestReplaceBid()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);
            
            depth.AddOrder(1236, 300, true);
            Assert.IsTrue(cc.VerifyBidChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, false, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, true);
            Assert.IsTrue(cc.VerifyBidChanged(false, true, false, false, false));
            cc.Reset();


            // Verify Levels 
            var bid0 = depth.Bids.First().Value;
            var bid1 = depth.Bids.ElementAt(1).Value;
            var bid2 = depth.Bids.ElementAt(2).Value;
            Assert.IsTrue(VerifyLevel(bid0, 1236, 1, 300));
            Assert.IsTrue(VerifyLevel(bid1, 1235, 2, 600));
            Assert.IsTrue(VerifyLevel(bid2, 1232, 1, 100));
            Assert.AreEqual(3, depth.Bids.Count);

            // Replace bid
            depth.ReplaceOrder(1235, 1237, 200, 200, true);

            // Verify Levels 
            bid0 = depth.Bids.First().Value;
            bid1 = depth.Bids.ElementAt(1).Value;
            bid2 = depth.Bids.ElementAt(2).Value;
            var bid3 = depth.Bids.ElementAt(3).Value;
            Assert.IsTrue(VerifyLevel(bid0, 1237, 1, 200));
            Assert.IsTrue(VerifyLevel(bid1, 1236, 1, 300));
            Assert.IsTrue(VerifyLevel(bid2, 1235, 1, 400));
            Assert.IsTrue(VerifyLevel(bid3, 1232, 1, 100));
            Assert.AreEqual(4, depth.Bids.Count);

            Assert.IsTrue(cc.VerifyBidChanged(true, true, true, true, false));
            cc.Reset();
        }

        [TestMethod]
        public void TestReplaceAsk()
        {
            var depth = new Depth();
            var cc = new ChangedChecker(depth);

            depth.AddOrder(1236, 300, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, false, false, false, false));
            cc.Reset();

            depth.AddOrder(1235, 200, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, false, false, false));
            cc.Reset();

            depth.AddOrder(1232, 100, false);
            Assert.IsTrue(cc.VerifyAskChanged(true, true, true, false, false));
            cc.Reset();

            depth.AddOrder(1235, 400, false);
            Assert.IsTrue(cc.VerifyAskChanged(false, true, false, false, false));
            cc.Reset();

            // Verify Levels 
            var ask0 = depth.Asks.First().Value;
            var ask1 = depth.Asks.ElementAt(1).Value;
            var ask2 = depth.Asks.ElementAt(2).Value;
            Assert.IsTrue(VerifyLevel(ask0, 1232, 1, 100));
            Assert.IsTrue(VerifyLevel(ask1, 1235, 2, 600));
            Assert.IsTrue(VerifyLevel(ask2, 1236, 1, 300));
            Assert.AreEqual(3, depth.Asks.Count);

            // Replace ask
            depth.ReplaceOrder(1235, 1237, 200, 200, false);

            // Verify Levels 
            ask0 = depth.Asks.First().Value;
            ask1 = depth.Asks.ElementAt(1).Value;
            ask2 = depth.Asks.ElementAt(2).Value;
            var ask3 = depth.Asks.ElementAt(3).Value;
            Assert.IsTrue(VerifyLevel(ask0, 1232, 1, 100));
            Assert.IsTrue(VerifyLevel(ask1, 1235, 1, 400));
            Assert.IsTrue(VerifyLevel(ask2, 1236, 1, 300));
            Assert.IsTrue(VerifyLevel(ask3, 1237, 1, 200));
            Assert.AreEqual(4, depth.Asks.Count);

            Assert.IsTrue(cc.VerifyAskChanged(false, true, false, true, false));
            cc.Reset();
        }
    }
}