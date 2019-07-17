﻿using Liquibook.NET.Types;

namespace Liquibook.NET.Book
{
    public interface IOrder
    {
        bool IsLimit { get; }
        bool IsBuy { get; }
        Price Price { get; }
        Price StopPrice { get; }
        Quantity OrderQty { get; }
        bool AllOrNone { get; }
        bool ImmediateOrCancel { get; }

        string OrderIdExt { get; set; } // External Id
        string OrderDomain { get; set; } // DT, EQ, FX

        // TODO: investigate methods at bottom of related cpp class
    }
}