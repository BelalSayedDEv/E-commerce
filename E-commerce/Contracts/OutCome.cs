namespace E_Commerce.Contracts
{
    public enum CartOutcome
    {
        ProductNotFound = 1,
        NotEnoughStock = 2,
        ItemAdded = 3,
        QuantityUpdated = 4,
    }
    public enum OrderOutcome
    {
        ProductDeleted = 1,
        NotEnoughStock = 2,
        CartItemsEmpty = 3,
        Ordersuccessfullycompleted = 4,
        Error = 5,
        SameStatus = 6,
        OrderNotFound = 7,

    }
}
