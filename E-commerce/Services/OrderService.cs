using E_Commerce.DTos.Order;
using E_Commerce.Model;
using E_Commerce.Repository;

namespace E_Commerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository orderRepository;
        private readonly ICartRepository cartRepository;
        private readonly ICartItemRepository cartItemRepository;
        private readonly IProductRepository productRepository;
        private readonly IOrderItemRepository orderItemRepository;
        private readonly ILogger<OrderService> logger;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository,
            ICartItemRepository cartItemRepository, IProductRepository productRepository,
            IOrderItemRepository orderItemRepository, ILogger<OrderService> logger)
        {
            this.orderRepository = orderRepository;
            this.cartRepository = cartRepository;
            this.cartItemRepository = cartItemRepository;
            this.productRepository = productRepository;
            this.orderItemRepository = orderItemRepository;
            this.logger = logger;
        }

        public List<OrderDto>? GetOrdersHistory(string userId)
        {
            var Orders = orderRepository.GetOrders().Where(o => o.UserId == userId);

            if (Orders == null)
                return null;

            return Orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalOrderPrice,
                Status = o.Status,
                Items = o.Items.Select(i => $"{i.Product?.Name} * {i.Quantity} = {i.Price * i.Quantity} ").ToList()

            }).ToList();

        }

        public List<OrderDto>? GetOrdersHistoryForAdmin()
        {
            var Orders = orderRepository.GetOrders();

            if (Orders == null)
                return null;

            return Orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalOrderPrice,
                Status = o.Status,
                Items = o.Items.Select(i => $"{i.Product?.Name} * {i.Quantity} = {i.Price * i.Quantity} ").ToList()

            }).ToList();

        }

        public async Task<OrderDto?> MakeOrder(string userId)
        {

            var context = orderRepository.Context();

            using var Transactions = await context.Database.BeginTransactionAsync();

            // Phase Validation 

            var cart = cartRepository.GetCartByUserId(userId);

            if (cart == null)
            {
                logger.LogWarning("There No Cart For user {UserID} ", userId);
                return null;
            }


            var cartItems = cartItemRepository.GetCartItemsByCartId(cart.Id);

            if (cartItems.Count == 0)
            {
                logger.LogWarning("Cart items is empty for {CartId}", cart.Id);
                return null;
            }



            foreach (var item in cartItems)
            {
                var product = await productRepository.GetProductByIdAsync(item.ProductID);

                if (product == null)
                {
                    logger.LogWarning(
                        "Product {ProductId} not found",
                        item.ProductID);

                    return null;
                }

                if (product != null)
                {
                    if (product.Quantity < item.Quantity)
                    {
                        logger.LogWarning("Empty stock {ProductId} ,Available {Available},Requested {Requested}",
                            product.Id, product.Quantity, item.Quantity);
                        return null;
                    }
                }
            }


            // Excution Phase

            Order order = new Order();

            order.UserId = userId;
            order.OrderDate = DateTime.Now;


            var orderWithId = orderRepository.Add(order);

            List<string> strings = new List<string>();


            double total = 0;

            foreach (var item in cartItems)
            {
                var product = await productRepository.GetProductByIdAsync(item.ProductID);


                if (product == null)
                {
                    logger.LogWarning(
                        "Product {ProductId} not found",
                        item.ProductID);

                    return null;
                }

                OrderItem orderItem = new OrderItem();

                orderItem.Order = order;
                orderItem.Quantity = item.Quantity;
                orderItem.ProductId = item.ProductID;
                orderItem.Price = product.Price;

                orderItemRepository.Add(orderItem);

                strings.Add($"{product.Name} * {item.Quantity} = {product.Price * item.Quantity} ");
                total += product.Price * item.Quantity;

                product.Quantity -= item.Quantity;

            }

            order.TotalOrderPrice = total;

            foreach (var item in cartItems)
                cartItemRepository.RemoveCartItem(item.Id);

            try
            {
                orderRepository.Save();
                await Transactions.CommitAsync();

            }
            catch (Exception ex)
            {
                //await Transactions.RollbackAsync();  // explicit — same thing happens via using
                //throw;  as memory
                logger.LogError(ex, "Faild to save order in database for user {UserId}", userId);
                return null;
            }

            logger.LogInformation("Order {OrderId} created. User: {UserId}, Total: {Total}", order.Id, userId, total);

            OrderDto orderDto = new OrderDto();
            orderDto.Id = orderWithId.Id;
            orderDto.OrderDate = DateTime.Now;
            orderDto.TotalPrice = total;
            orderDto.Status = order.Status;
            orderDto.Items = strings;

            return orderDto;
        }


        public OrderDto? UpdateStatus(int Id, string status)
        {
            var order = orderRepository.GetOrderById(Id);

            if (order == null)
            {
                logger.LogWarning("there no order for {OrderId}", Id);
                return null; // NotFound
            }
            if (order.Status == status)
            {
                logger.LogWarning("Error: admin update old status: {Status}  , new status: {New}", order.Status, status);
            }
            order.Status = status;

            orderRepository.Save();
            logger.LogInformation("Status order updated successfuly to {NewStatus} by admin", status);
            OrderDto dto = new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalOrderPrice,
                Status = order.Status,
                Items = order.Items.Select(i => $"{i.Product?.Name} * {i.Quantity} = {i.Price * i.Quantity}").ToList()
            };

            return dto;
        }
    }
}
