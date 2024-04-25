using Common.Constants;
using Common.Exception;
using Data.Entities;
using Data.Entities.User;
using Data.IRepository;
using Data.ViewModels.Cart;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Service.IServices;

namespace Service.Services;

public class CartService : ICartService
{
    private readonly IRepository<Cart> _cartRepo;
    private readonly IRepository<Book> _bookRepo;


    public CartService(IRepository<Cart> cartRepo, IRepository<Book> bookRepo)
    {
        _cartRepo = cartRepo;
        _bookRepo = bookRepo; 
    }

    public async Task<IEnumerable<Cart>> GetAll()
    {
        return await _cartRepo.GetAllAsync(null, o => o.OrderByDescending(c => c.CreateAt));
    }

    public async Task<Cart> GetById(string id)
    {
        return await _cartRepo.GetByIdAsync(id); 
    }

    public Task<Cart> Create(string userId, Cart model, string stallId, string bookId)
    {
        throw new NotImplementedException();
    }

    public async Task<Cart> Update(string id, string userId, Cart model)
    {
        var cartItem = await GetById(id); 
        if(cartItem == null) throw new AppException("Item not found");

        cartItem.Quantity = model.Quantity;
        cartItem.UserId = model.UserId; 
        cartItem.IsSelected = model.IsSelected;
        cartItem.UpdateBy = userId; 
        _cartRepo.Update(cartItem);
        await _cartRepo.SaveChangesAsync();
        return cartItem; 
    }

    public async Task Delete(string id)
    {
        var stall = await _cartRepo.GetByIdAsync(id);
        if (stall == null)
            throw new AppException("Cart not exit"); 
        _cartRepo.Remove(stall);
        await _cartRepo.SaveChangesAsync(); 
    }
    
    public Task<IEnumerable<Cart>> GetByUserId(string userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Cart>> GetAllByUserId(string userId)
    {
        var listCart = await _cartRepo.GetAllAsync(c => c.UserId == userId,
            o => o.OrderByDescending(c => c.CreateAt), c => c.Book.Images);
        
        return listCart;
    }

    public async Task<Cart> ReduceQuantity(string id, string userId)
    {
        var cart = await _cartRepo.GetFirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, c => c.Book);
        if (cart.Quantity == 1) throw new AppException("Cart item remaining 1. Can not reduce item anymore"); 
        cart.Quantity -= 1;
        cart.Total = cart.Quantity * cart.Book.Price; 
        _cartRepo.Update(cart);
        await _cartRepo.SaveChangesAsync();
        return cart; 
    }
    public async Task<Cart> IncrementQuantity(string id, string userId)
    {
        var cart = await _cartRepo.GetFirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, c => c.Book);
        cart.Quantity += 1;
        cart.Total = cart.Quantity * cart.Book.Price; 
        _cartRepo.Update(cart);
        await _cartRepo.SaveChangesAsync();
        return cart; 
    }


    public async Task<Cart> AddToCart(string bookId, string userId, int quantity, int total)
    {
        var book = await _bookRepo.GetFirstOrDefaultAsync(b => b.Id == bookId, include => include.Post);
        var itemExists = await _cartRepo.GetFirstOrDefaultAsync(c => c.BookId == bookId && c.UserId == userId);
        if (itemExists != null)
        {
            itemExists.Quantity = itemExists.Quantity + quantity;
            itemExists.Total = itemExists.Total + total;
            await _cartRepo.SaveChangesAsync();
            return itemExists;
        }

        var newCart = new Cart()
        {
            BookId = bookId,  
            UserId = userId,
            Quantity = quantity,
            CreateAt = DateTime.Now,
            CreatedBy = userId, 
            Total = total,
            StallId = book.Post.StallId,
            IsSelected = StringEnum.SelectedItemStatus.NonSelected 
        };
        await _cartRepo.AddAsync(newCart);
        await _cartRepo.SaveChangesAsync();
        return newCart;
    }

    public async Task<Cart> RemoveByUserId(string id, string userId)
    {
        var cartItem = await _cartRepo.GetFirstOrDefaultAsync(c => c.Id == id  && c.UserId == userId);
        if (cartItem == null)
        {
            throw new AppException("Cart item doesn't exit"); 
        }
        _cartRepo.Remove(cartItem); 
        await _cartRepo.SaveChangesAsync();
        return cartItem;
        
    }

    public async Task<IEnumerable<Cart>> GetAllCartSelected(string userId)
    {
        var itemSelected = await _cartRepo.GetAllAsync(c => c.UserId == userId && c.IsSelected == StringEnum.SelectedItemStatus.Selected , null, i => i.Book.Images);
        return itemSelected; 
    }

    public async Task<IEnumerable<Cart>> RemoveListCartItem(string userId)
    {
        var carts = await _cartRepo.GetAllAsync(c =>
            c.UserId == userId && c.IsSelected == StringEnum.SelectedItemStatus.Selected);
        foreach (var item in carts)
        {
            _cartRepo.Remove(item); 
            await _cartRepo.SaveChangesAsync(); 
        }
        return carts; 
    }
}