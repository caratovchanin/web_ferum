using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using webFerum.Models;
using webFerum.Models.AppContext;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System;
using System.Runtime.CompilerServices;
using webFerum.Utils;

namespace webFerum.Service
{
    public class TgService
    {
        private IDistributedCache cache;
        private WebFerumContext context;
        private UserService uService;
        private ItemService iService;
        public static TgService service;

        ITelegramBotClient botClient = new TelegramBotClient("7072474601:AAF4wfp7tk5RFqE49Dp-iRzRIPTojOwqbyk");

        public TgService(WebFerumContext ferrumContext, IDistributedCache distributedCache, UserService userService, ItemService itemService)
        {
            cache = distributedCache;
            context = ferrumContext;
            uService = userService;
            iService = itemService;
            service = this;
            tgBot.Start();
        }

        public async void AnswerToClientAsync(Client client)
        {
            var queueItem = emplQueue.Dequeue();
            if (emplList.Contains(queueItem.Item1))
            {
                AnswerToClientAsync(client);
                return;
            }
            else
            {
                emplQueue.Enqueue(queueItem);
                webFerum.Models.AppContext.User usClient = await uService.GetEmployeeAsync(client.IdUser);
                webFerum.Models.AppContext.Item item = await iService.GetItemAsync(client.IdItem);
                await botClient.SendTextMessageAsync(queueItem.Item1, $"{client.Message} Продукт - {item.Name} Данные - {usClient.Name} {usClient.Surname} {usClient.Lastname} {usClient.Number} {usClient.Email}");
            }
           
        }

        private static Queue<(long ,webFerum.Models.AppContext.User )> emplQueue = new Queue<(long, Models.AppContext.User)>();
        private static List<long> emplList = new List<long>();
        public Task tgBot = new Task( async () => {

            
            ReceiverOptions receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[]
                {
                    UpdateType.Message,
                },

                ThrowPendingUpdates = true,
            };

            using var cts = new CancellationTokenSource();



            service.botClient.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions, cts.Token);

            await Task.Delay(-1);
        });

        private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            
            
            try
            {
                
                switch (update.Type)
                {
                    case UpdateType.Message:
                    {
                        switch(update.Message.Type) 
                        {
                            case MessageType.Text:
                            {
                                        if (update.Message.Text.Split()[0] == "/start")
                                        {
                                            string email = update.Message.Text.Split()[1];
                                            string pass = Utils.Encrypt.sha256(update.Message.Text.Split()[2]);

                                            var userMod = new UserModel()
                                            {
                                                Email = email,
                                                Password = pass
                                            };

                                            webFerum.Models.AppContext.User us = await service.uService.GetEmployeeAsync(userMod);
                                            if (us.Role.Policy == "Operator" || us.Role.Policy == "Admin")
                                            {
                                                if (emplList.Contains(update.Message.Chat.Id) == true)
                                                {
                                                    emplList.Remove(update.Message.Chat.Id);
                                                }
                                                emplQueue.Enqueue((update.Message.Chat.Id, us));
                                                
                                                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Вы успешно встали на смену");
                                            }
                                            else
                                                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Не удалось подключиться");

                                        }
                                        else if (update.Message.Text == "/end")
                                        {
                                            emplList.Add(update.Message.Chat.Id);
                                        }
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
        {
            
            var ErrorMessage = error switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => error.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }   
}
