using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Builder;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Sitecore.Commerce.Plugin.CustomizableProduct.Pipelines.Blocks
{
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        private readonly IPersistEntityPipeline _persistEntityPipeline;

        public ConfigureServiceApiBlock(IPersistEntityPipeline persistEntityPipeline)
        {
            _persistEntityPipeline = persistEntityPipeline;
        }

        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{base.Name}: The argument can not be null");

            var cartLineConfiguration = modelBuilder.Action("SetCartLineProperties");
            cartLineConfiguration.Parameter<string>("cartId");
            cartLineConfiguration.Parameter<Models.Properties>("properties");
            cartLineConfiguration.ReturnsFromEntitySet<CommerceCommand>("Commands");
            return Task.FromResult(modelBuilder);
        }
    }
}
