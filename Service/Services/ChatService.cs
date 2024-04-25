using Backend.Dtos.ChatDtos;
using Common.Exception;
using Data.DbContext;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Service.IServices;

namespace Service.Services;

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;
    private readonly IImageService _imageServices;

    public ChatService(
        ApplicationDbContext context,
        IImageService imageServices
    )
    {
        _context = context;
        _imageServices = imageServices;
    }

    public async Task<List<GetChatByUserIdResponse>> GetChatByUserId(string id)
    {
        var chatUsers = await _context.ChatUsers
            .Where(cu => cu.UserId == id)
            .ToListAsync();

        var chatIds = chatUsers.Select(cu => cu.ChatId).Distinct().ToList();

        var chatUsersContainChatWithUsers = await _context.ChatUsers
            .Where(cu => chatIds.Contains(cu.ChatId))
            .ToListAsync();

        var chats = await _context.Chats
            .Where(c => chatIds.Contains(c.Id))
            .ToListAsync();

        var userIds = chatUsersContainChatWithUsers.Select(cu => cu.UserId).Distinct().ToList();

        var users = await _context.Users
            .Where(a => userIds.Contains(a.Id))
            .ToListAsync();

        var messages = await _context.ChatMessages
            .OrderByDescending(x => x.CreateAt)
            .Where(cm => chatIds.Contains(cm.ChatId))
            .Take(20)
            .ToListAsync();

        var res = new List<GetChatByUserIdResponse>();

        foreach (var chat in chats)
        {
            var chatUser = chatUsers.FirstOrDefault(cu => cu.UserId == id && cu.ChatId == chat.Id);

            var userIdsChatWith = chatUsersContainChatWithUsers
                .Where(cu => cu.ChatId == chat.Id && cu.UserId != id)
                .Select(cu => cu.UserId)
                .ToList();

            var usersChatWith = users
                .Where(u => userIdsChatWith.Contains(u.Id))
                .ToList();

            var userResponseList = usersChatWith
                .Select(user => new UserResponse
                {
                    Id = user.Id,
                    Avatar = user.Avatar,
                    FullName = user.FullName,
                    Email = user.Email,
                    ChatUser = new ChatUserResponse
                    {
                        ChatId = chatUsersContainChatWithUsers
                            .FirstOrDefault(cu => cu.ChatId == chat.Id && cu.UserId == user.Id)?
                            .ChatId ?? "",
                        UserId = user.Id,
                    }
                })
                .ToList();

            var messageResponseList = messages
                .Where(m => m.ChatId == chat.Id)
                .Select(m => new MessageResponse
                {
                    Id = m.Id,
                    Type = m.Type,
                    Message = m.Message,
                    ChatId = m.ChatId,
                    FromUserId = m.FromUserId,
                    CreatedAt = m.CreateAt.Value,
                    UpdatedAt = m.CreateAt.Value,
                    User = new UserInMessage
                    {
                        Id = m.FromUserId,
                        Avatar = users.FirstOrDefault(u => u.Id == m.FromUserId)?.Avatar,
                        FullName = users.FirstOrDefault(u => u.Id == m.FromUserId)?.FullName,
                        Email = users.FirstOrDefault(u => u.Id == m.FromUserId)?.Email,
                    }
                })
                .ToList();

            if (chatUser != null)
            {
                var result = new GetChatByUserIdResponse
                {
                    Id = chat.Id,
                    Type = chat.Type,
                    ChatUser = new ChatUserResponse
                    {
                        UserId = chatUser.UserId,
                        ChatId = chatUser.ChatId,
                    },
                    Users = userResponseList,
                    Messages = messageResponseList,
                };
                res.Add(result);
            }
        }

        return res;
    }


    public async Task<CreateChatResponse> Create(string partnerId, string userId)
    {
        if (partnerId == userId)
        {
            throw new AppException("Cannot Chat with yourself!");
        }

        var chatUsers = await _context.ChatUsers
            .Where(cu => cu.UserId == partnerId || cu.UserId == userId)
            .ToListAsync();

        var chatIdInChatUserOfPartner = chatUsers
            .Where(cu => cu.UserId == partnerId)
            .Select(cu => cu.ChatId);

        var chatIdInChatUserOfUser = chatUsers
            .Where(cu => cu.UserId == userId)
            .Select(cu => cu.ChatId);

        var isChatIdOfUserAndPartnerIntersect = chatIdInChatUserOfPartner.Intersect(chatIdInChatUserOfUser).Any();

        var isChatTypeDual = isChatIdOfUserAndPartnerIntersect
            ? await _context.Chats
                .Where(c => chatIdInChatUserOfPartner.Intersect(chatIdInChatUserOfUser).Contains(c.Id) &&
                            c.Type == "dual")
                .AnyAsync()
            : false;

        var isUserAndPartnerAlreadyChat = isChatIdOfUserAndPartnerIntersect && isChatTypeDual;

        if (isUserAndPartnerAlreadyChat)
        {
            throw new AppException("Chat with this user already exists!");
        }

        var chat = new Chat
        {
            Type = "dual"
        };

        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        var chatUsersToAdd = new List<ChatUser>
        {
            new ChatUser { UserId = userId, ChatId = chat.Id },
            new ChatUser { UserId = partnerId, ChatId = chat.Id }
        };

        _context.ChatUsers.AddRange(chatUsersToAdd);
        await _context.SaveChangesAsync();

        var userInfos = await _context.Users
            .Where(a => a.Id == userId || a.Id == partnerId)
            .ToListAsync();

        var userInfo = userInfos.FirstOrDefault(a => a.Id == userId);
        var partnerInfo = userInfos.FirstOrDefault(a => a.Id == partnerId);

        var forCreator = new CreateChatResponseModel
        {
            Id = chat.Id,
            Type = "dual",
            ChatUser = new
            {
                chatId = chat.Id,
                userId = userId,
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            },
            Users = new List<UserInMessage>
            {
                new UserInMessage
                {
                    Id = partnerInfo.Id,
                    Avatar = partnerInfo.Avatar,
                    FullName = partnerInfo.FullName,
                    Email = partnerInfo.Email,
                }
            }
        };

        var forReceiver = new CreateChatResponseModel
        {
            Id = chat.Id,
            Type = "dual",
            ChatUser = new
            {
                chatId = chat.Id,
                userId = partnerId,
                createdAt = DateTime.UtcNow,
                updatedAt = DateTime.UtcNow
            },
            Users = new List<UserInMessage>
            {
                new UserInMessage
                {
                    Id = userInfo.Id,
                    Avatar = userInfo.Avatar,
                    FullName = userInfo.FullName,
                    Email = userInfo.Email
                }
            },
        };

        return new CreateChatResponse
        {
            CreateChatResponseModels = new List<CreateChatResponseModel> { forCreator, forReceiver }
        };
    }


    public async Task<object> Messages(string id, int page = 1)
    {
        var limit = 10;
        var offset = page > 1 ? page * limit : 0;

        var totalMessages = await _context.ChatMessages
            .CountAsync(cm => cm.ChatId == id);

        var totalPages = Math.Ceiling(Convert.ToDecimal(totalMessages) / limit);

        var messagesQuery = _context.ChatMessages
            .Where(cm => cm.ChatId == id)
            .OrderByDescending(cm => cm.CreateAt)
            .Skip(offset)
            .Take(limit);

        var messages = await messagesQuery.ToListAsync();

        if (page > totalPages)
        {
            return new { data = new { messages = new List<MessageResponse>() } };
        }

        var fromUserIds = messages.Select(cm => cm.FromUserId).ToList();

        var userInfos = await _context.Users
            .Where(a => fromUserIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id);

        var messageResponses = messages
            .Select(m => new MessageResponse
            {
                Id = m.Id,
                Type = m.Type,
                Message = m.Message,
                ChatId = m.ChatId,
                FromUserId = m.FromUserId,
                CreatedAt = m.CreateAt.Value,
                UpdatedAt = m.CreateAt.Value,
                User = userInfos.TryGetValue(m.FromUserId, out var userInfo)
                    ? new UserInMessage
                    {
                        Id = userInfo.Id,
                        Avatar = userInfo.Avatar,
                        FullName = userInfo.FullName,
                        Email = userInfo.Email,
                    }
                    : null
            })
            .ToList();

        return new { messages = messageResponses, pagination = new { page = page, totalPages = totalPages } };
    }

    public async Task<object> Delete(string chatId)
    {
        var chatUsersToDelete = await _context.ChatUsers
            .Where(cu => cu.ChatId == chatId)
            .ToListAsync();

        _context.ChatUsers.RemoveRange(chatUsersToDelete);

        var messagesToDelete = await _context.ChatMessages
            .Where(cm => cm.ChatId == chatId)
            .ToListAsync();

        _context.ChatMessages.RemoveRange(messagesToDelete);

        var chatsToDelete = await _context.Chats
            .Where(c => c.Id == chatId)
            .ToListAsync();

        _context.Chats.RemoveRange(chatsToDelete);

        await _context.SaveChangesAsync();

        return new { ChatId = chatId, NotifyUsers = chatUsersToDelete.Select(cu => cu.UserId) };
    }

    public object UpLoadImage(Stream image)
    {
        var uploadResult = _imageServices.UploadFile(image, Guid.NewGuid().ToString());
        return new { Url = uploadResult.Url.ToString() };
    }
}