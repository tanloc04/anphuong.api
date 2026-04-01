using anphuong.Core.Domains.DTOs;
using anphuong.Core.Domains.DTOs.RequestDTOs.Cart;
using anphuong.Core.Domains.Entities;
using anphuong.Core.Domains.Objects;
using anphuong.Core.Exceptions;
using anphuong.Core.Interfaces.Repositories;
using anphuong.Core.Interfaces.Services;
using anphuong.Repository.Repositories;
using Mapster;

namespace anphuong.Service
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;
        

        public CartService(ICartRepository repository)
        {
            _repository = repository;

            TypeAdapterConfig<CartItem, CartItemDTO>.NewConfig()
                .Map(dest => dest.ProductName, src => src.Product.Name)
                .Map(dest => dest.Price, src => src.Product.Price)
                .Map(dest => dest.ProductImage, src =>
                    src.Variant != null && !string.IsNullOrEmpty(src.Variant.VariantImage)
                        ? src.Variant.VariantImage
                        : (src.Product.DetailImage != null ? src.Product.DetailImage.Image1 : ""))
                .Map(dest => dest.VariantDisplay, src =>
                    src.Variant != null
                        ? $"{src.Variant.Color.Name} - {src.Variant.Material.Name}"
                        : "")
                .Map(dest => dest.MaxStock, src =>
                    (src.Variant != null && src.Variant.Inventory != null)
                        ? src.Variant.Inventory.QuantityInStock
                        : 0);
        }

        public async Task SyncCartAsync(int userId, List<SyncCartRequestDTO> localItems)
        {
            if (localItems == null || !localItems.Any()) return;

            var cart = await _repository.GetAsync(userId);

            bool isNewCart = false;

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CartItems = new List<CartItem>()
                };
                isNewCart = true;
            }

            foreach (var localItem in localItems)
            {
                var existingItem = cart.CartItems.FirstOrDefault(ci =>
                    ci.ProductId == localItem.ProductId &&
                    ci.VariantId == localItem.VariantId &&
                    !ci.IsDeleted);

                if (existingItem != null)
                {
                    existingItem.Quantity += localItem.Quantity;
                    existingItem.UpdatedAt = DateTime.Now;
                }
                else
                {
                    cart.CartItems.Add(new CartItem
                    {
                        ProductId = localItem.ProductId,
                        VariantId = localItem.VariantId,
                        Quantity = localItem.Quantity,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }
            }

            if (isNewCart)
            {
                await _repository.AddAsync(cart);
            }
            else
            {
                cart.UpdatedAt = DateTime.Now;
                _repository.Update(cart);
            }
        }

        public async Task<CartDTO> GetCartAsync(int userId)
        {
            var cart = await _repository.GetCartByUserIdAsync(userId);
            if (cart == null) return null;

            return cart.Adapt<CartDTO>();
        }

        public async Task AddToCartAsync(int userId, SyncCartRequestDTO request)
        {
            var cart = await _repository.GetCartByUserIdAsync(userId);
            bool isNewCart = false;

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CartItems = new List<CartItem>()
                };
                isNewCart = true;
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci =>
                ci.ProductId == request.ProductId &&
                ci.VariantId == request.VariantId &&
                !ci.IsDeleted);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                existingItem.UpdatedAt = DateTime.Now;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = request.ProductId,
                    VariantId = request.VariantId,
                    Quantity = request.Quantity,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }

            if (isNewCart)
            {
                await _repository.AddAsync(cart);
            }
            else
            {
                cart.UpdatedAt = DateTime.Now;
                _repository.Update(cart);
            }
        }

        public async Task UpdateCartItemAsync(int userId, int cartItemId, int quantity)
        {
            var cart = await _repository.GetCartByUserIdAsync(userId);
            if (cart == null) throw new BusinessException(ErrorDetails.DEFAULT);

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId && !ci.IsDeleted);
            if (item == null) throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            if (quantity <= 0)
            {
                item.IsDeleted = true;
            }
            else
            {
                item.Quantity = quantity;
            }

            item.UpdatedAt = DateTime.Now;
            cart.UpdatedAt = DateTime.Now;

            if (!_repository.Update(cart))
            {
                throw new BusinessException(ErrorDetails.DEFAULT);
            }
        }

        public async Task RemoveCartItemAsync(int userId, int cartItemId)
        {
            var cart = await _repository.GetCartByUserIdAsync(userId);
            if (cart == null) throw new BusinessException(ErrorDetails.DEFAULT);

            var item = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId && !ci.IsDeleted);
            if (item == null) throw new BusinessException(ErrorDetails.ID_NOT_FOUND);

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.Now;
            cart.UpdatedAt = DateTime.Now;

            if (!_repository.Update(cart))
            {
                throw new BusinessException(ErrorDetails.DEFAULT);
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _repository.GetCartByUserIdAsync(userId);

            if (cart == null) return;

            var activeItems = cart.CartItems.Where(ci => !ci.IsDeleted).ToList();

            if (activeItems.Any())
            {
                foreach (var item in activeItems)
                {
                    item.IsDeleted = true;
                    item.UpdatedAt = DateTime.Now;
                }

                cart.UpdatedAt = DateTime.Now;

                if (!_repository.Update(cart))
                {
                    throw new BusinessException(ErrorDetails.DEFAULT);
                }
            }
        }
    }
}