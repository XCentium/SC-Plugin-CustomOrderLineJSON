using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines;
using Sitecore.Framework.Pipelines.Definitions.Extensions;

namespace Sitecore.Commerce.Plugin.CustomizableProduct
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            Action<SitecorePipelinesConfigBuilder> actionDelegate = c => c
                .ConfigurePipeline<IConfigureServiceApiPipeline>(
                    d =>
                    {
                        d.Add<Pipelines.Blocks.ConfigureServiceApiBlock>();
                    });

            services.Sitecore().Pipelines(actionDelegate);
            // Register commands too.
            services.RegisterAllCommands(assembly);
        }
    }
}
