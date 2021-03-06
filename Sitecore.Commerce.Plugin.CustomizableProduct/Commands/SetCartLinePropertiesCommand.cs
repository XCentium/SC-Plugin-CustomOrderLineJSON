﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.CustomizableProduct.Components;
using Sitecore.Commerce.Plugin.CustomizableProduct.Models;

namespace Sitecore.Commerce.Plugin.CustomizableProduct.Commands
{
    public class SetCartLinePropertiesCommand : CommerceCommand
    {

        /// <summary>
        /// 
        /// </summary>
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="persistEntityPipeline"></param>
        public SetCartLinePropertiesCommand(IServiceProvider serviceProvider,
            IPersistEntityPipeline persistEntityPipeline) : base(serviceProvider)
        {
            _persistEntityPipeline = persistEntityPipeline;
        }

        public async Task<Cart> Process(CommerceContext commerceContext, string cartId, CartLineProperties lineProperties, string baseUrl)
        {
            try
            {

                var cart = GetCart(cartId, commerceContext, baseUrl);
                if (cart == null)
                {
                    return null;
                }


                // Set the custom fields on the cartlines
                if (cart.Lines != null && cart.Lines.Any() && lineProperties != null &&
                    lineProperties.CartLineProperty.Any())
                {
                    foreach (var cartLineProperty in lineProperties.CartLineProperty)
                    {
                        var cartLineComponent = cart.Lines.FirstOrDefault(x => x.Id == cartLineProperty.CartLineId);
                        if (cartLineComponent != null)
                            cartLineComponent
                                .GetComponent<CartComponent>().Properties = cartLineProperty.Properties;
                    }
                }

                var result = await this._persistEntityPipeline.Run(new PersistEntityArgument(cart), commerceContext.GetPipelineContextOptions());

                return result.Entity as Cart;
            }
            catch (Exception e)
            {
                return await Task.FromException<Cart>(e);
            }
        }

        private Cart GetCart(string cartId, CommerceContext commerceContext, string baseUrl)
        {
            var shopName = commerceContext.CurrentShopName();
            var shopperId = commerceContext.CurrentShopperId();
            var customerId = commerceContext.CurrentCustomerId();
            var environment = commerceContext.Environment.Name;

            var url = string.Format(Constants.Settings.EndpointUrl, baseUrl, cartId);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.Settings.AppJson));
            client.DefaultRequestHeaders.Add(Constants.Settings.ShopName, shopName);
            client.DefaultRequestHeaders.Add(Constants.Settings.ShopperId, shopperId);
            client.DefaultRequestHeaders.Add(Constants.Settings.Language, "en-US");
            client.DefaultRequestHeaders.Add(Constants.Settings.Environment, environment);
            client.DefaultRequestHeaders.Add(Constants.Settings.CustomerId, customerId);
            client.DefaultRequestHeaders.Add(Constants.Settings.Currency, commerceContext.CurrentCurrency());
            client.DefaultRequestHeaders.Add(Constants.Settings.Roles, Constants.Settings.CartRoles);


            try
            {
                var cart = new Cart();

                var response = client.GetAsync(url).Result;

                if (response != null)
                {

                    var task = response.Content.ReadAsStreamAsync().ContinueWith(t =>
                    {
                        var stream = t.Result;
                        using (var reader = new StreamReader(stream))
                        {
                            var responseValue = reader.ReadToEnd();
                            cart = JsonConvert.DeserializeObject<Cart>(responseValue);
                        }
                    });

                    task.Wait();
                }

                client.Dispose();
                return cart;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                client.Dispose();
                return null;
            }

        }

    }
}
