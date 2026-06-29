using E_Commerce.Contracts;
using E_Commerce.DTos.Order;
using E_Commerce.Model;
using E_Commerce.Repository;

namespace E_Commerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly ICartRepository cartRepository;
        private readonly IProductRepository productRepository;
        private readonly ILogger<OrderService> logger;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, IProductRepository productRepository,
            ILogger<OrderService> logger)
        {
            this.orderRepository = orderRepository;
            this.cartRepository = cartRepository;
            this.productRepository = productRepository;
            this.logger = logger;
        }




        public async Task<OrderResult> MakeOrder(string userId)
        {
            var context = orderRepository.Context();

            using var Transactions = await context.Database.BeginTransactionAsync();

            //Phase Validation 

            var cart = cartRepository.GetCartByUserId(userId);

            if (cart == null)
            {
                logger.LogWarning("There No Cart For user {UserID} ", userId);
                return new OrderResult()
                {
                    Outcome = OrderOutcome.CartItemsEmpty,
                    Message = "Cart items is empty"
                };
            }

            var Items = await cartRepository.GetCartItems(cart.Id);

            if (!Items.Any())
            {
                logger.LogWarning("Cart items is empty for {CartId}", cart.Id);

                return new OrderResult()
                {
                    Outcome = OrderOutcome.CartItemsEmpty,
                    Message = "Cart items is empty"
                };
            }



            foreach (var item in Items)
            {
                if (item.Product == null)
                {
                    logger.LogWarning(
                        "Product {ProductId} not found", item.ProductID);
                    return new OrderResult()
                    {
                        Outcome = OrderOutcome.ProductDeleted,
                        ProductName = $"{item.Product.Name}",

                    };
                }


                if (item.Quantity > item.Product.Quantity)
                {
                    logger.LogWarning("Empty stock {ProductId} ,Available {Available},Requested {Requested}",
                        item.Product.Id, item.Product.Quantity, item.Quantity);
                    return new OrderResult()
                    {
                        Outcome = OrderOutcome.NotEnoughStock,
                        ProductName = item.Product.Name,
                        AvailableStock = item.Product.Quantity,
                    };
                }

            }


            // Excution Phase

            Order order = new Order();

            order.UserId = userId;
            order.OrderDate = DateTime.Now;


            await orderRepository.AddNewOrder(order);

            List<string> strings = new List<string>();

            foreach (var item in Items)
            {

                OrderItem orderItem = new OrderItem();

                orderItem.Order = order;
                orderItem.Quantity = item.Quantity;
                orderItem.ProductId = item.ProductID;
                orderItem.Price = item.Product.Price;

                await orderRepository.AddNewOrderItem(orderItem);

                strings.Add($"{item.Product.Name} * {item.Quantity} = {item.Product.Price * item.Quantity} ");
                item.Product.Quantity -= item.Quantity;

            }

            order.TotalOrderPrice = Items.Sum(i => i.Product.Price * i.Quantity);

            Items.Clear();

            try
            {
                await orderRepository.Save();
                await Transactions.CommitAsync();

            }
            catch (Exception ex)
            {
                //await Transactions.RollbackAsync();  // explicit — same thing happens via using
                //throw;  as memory

                logger.LogError(ex, "Faild to save order in database for user {UserId}", userId);
                return new OrderResult()
                {
                    Outcome = OrderOutcome.Error,
                    Message = ex.Message,
                };
            }

            logger.LogInformation("Order {OrderId} created. User: {UserId}, Total: {Total}", order.Id, userId, order.TotalOrderPrice);

            OrderDto orderDto = new OrderDto();

            orderDto.Id = order.Id;
            orderDto.OrderDate = DateTime.Now;
            orderDto.TotalPrice = order.TotalOrderPrice;
            orderDto.Status = order.Status;
            orderDto.Items = strings;

            return new OrderResult()
            {
                Outcome = OrderOutcome.Ordersuccessfullycompleted,
                OrderDto = orderDto
            };

        }

        public async Task<List<OrderDto>> GetOrdersHistory(string userId)
        {
            var Orders = await orderRepository.GetOrdersByUserId(userId);

            return Orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalOrderPrice,
                Status = o.Status,
                Items = o.Items.Select(i => $"{i.Product?.Name} * {i.Quantity} = {i.Price * i.Quantity} ").ToList()

            }).ToList();

        }


        public async Task<List<OrderDto>> GetOrdersHistoryForAdmin()
        {
            var Orders = await orderRepository.GetOrders();

            return Orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalOrderPrice,
                Status = o.Status,
                Items = o.Items.Select(i => $"{i.Product?.Name} * {i.Quantity} = {i.Price * i.Quantity} ").ToList()

            }).ToList();

        }


        public async Task<OrderResult> UpdateStatus(int Id, string status)
        {
            var order = await orderRepository.GetOrderById(Id);

            if (order == null)
            {
                logger.LogWarning("there no order for {OrderId}", Id);
                return new OrderResult()
                {
                    Outcome = OrderOutcome.OrderNotFound,
                    Message = "Order Not Found",

                };
            }

            if (order.Status == status)
            {
                logger.LogWarning("Error: admin update old status: {Status}  , new status: {New}", order.Status, status);
                return new OrderResult()
                {
                    Outcome = OrderOutcome.SameStatus,
                    Message = "Same status no change here",

                };
            }
            order.Status = status;

            await orderRepository.Save();
            logger.LogInformation("Status order updated successfuly to {NewStatus} by admin", status);

            OrderDto dto = new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalOrderPrice,
                Status = order.Status,
                Items = order.Items.Select(i => $"{i.Product?.Name} * {i.Quantity} = {i.Price * i.Quantity}").ToList()
            };

            return new OrderResult()
            {
                Outcome = OrderOutcome.Ordersuccessfullycompleted,
                OrderDto = dto,

            };
        }
    }
}
