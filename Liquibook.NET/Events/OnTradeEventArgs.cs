using Liquibook.NET.Book;
using Liquibook.NET.Types;

namespace Liquibook.NET.Events
{
    public class OnTradeEventArgs
    {
        public OrderBook Book { get; }
        public Quantity Quantity { get; }
        public decimal Cost { get; }

        public OnTradeEventArgs(OrderBook book, Quantity quantity, decimal cost)
        {
            Book = book;
            Quantity = quantity;
            Cost = cost;
        }
    }
}