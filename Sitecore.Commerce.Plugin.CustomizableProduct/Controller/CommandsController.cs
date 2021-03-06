﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.OData;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.CustomizableProduct.Commands;
using Sitecore.Commerce.Plugin.CustomizableProduct.Models;

namespace Sitecore.Commerce.Plugin.CustomizableProduct.Controller
{
    /// <summary>
    /// 
    /// </summary>
    public class CommandsController : CommerceController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="globalEnvironment"></param>
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment) : base(serviceProvider, globalEnvironment)
        {

        }

        // 
        [HttpPost]
        [Route("SetCartLineProperties()")]
        public async Task<IActionResult> SetCartLineProperties([FromBody] ODataActionParameters value)
        {

            if (!this.ModelState.IsValid)
                return (IActionResult)new BadRequestObjectResult(this.ModelState);
            
            if (!value.ContainsKey(Constants.Settings.CartId)) return (IActionResult)new BadRequestObjectResult((object)value);
            var id = value[Constants.Settings.CartId];

            if (string.IsNullOrEmpty(id?.ToString())) return (IActionResult)new BadRequestObjectResult((object)value);

            var cartId = id.ToString();

            var cartLineProperties = new CartLineProperties();
            if (value.ContainsKey(Constants.Settings.CartLineProperties))
            {
                var cartlinePropObj = value[Constants.Settings.CartLineProperties];

                if (!string.IsNullOrEmpty(cartLineProperties?.ToString()))
                {
                    cartLineProperties = JsonConvert.DeserializeObject<CartLineProperties>(cartlinePropObj.ToString());
                }
            }

            var command = this.Command<SetCartLinePropertiesCommand>();
            await Task.Delay(1);
            var runCommand = await command.Process(this.CurrentContext, cartId, cartLineProperties, $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}");

            return (IActionResult)new ObjectResult((object)runCommand);
        }

    }
}

