using Liquibook.NET.Book;
using Liquibook.NET.Types;

namespace Liquibook.NET.Events
{
    public class OnFillEventArgs
    {
        public IOrder Order { get; }
        public IOrder MatchedOrder { get; }
        public Quantity FillQuantity { get; }
        public decimal FillCost { get; }
        public bool InboundOrderFilled { get; }
        public bool MatchedOrderFilled { get; }

        public OnFillEventArgs(IOrder order, IOrder matchedOrder, Quantity fillQuantity, decimal fillCost,
            bool inboundOrderFilled, bool matchedOrderFilled)
        {
            Order = order;
            MatchedOrder = matchedOrder;
            FillQuantity = fillQuantity;
            FillCost = fillCost;
            InboundOrderFilled = inboundOrderFilled;
            MatchedOrderFilled = matchedOrderFilled;
        }
    }
}