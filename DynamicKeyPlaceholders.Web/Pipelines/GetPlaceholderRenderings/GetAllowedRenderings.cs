using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetPlaceholderRenderings;

namespace DragoniaStudios.SC.Pipelines.GetPlaceholderRenderings
{
    public class GetAllowedRenderings : Sitecore.Pipelines.GetPlaceholderRenderings.GetAllowedRenderings
    {
        /// <summary>
        /// This is a method based on the original Sitecore method to allow for dynamic placeholder keys
        /// </summary>
        /// <param name="args">The Placeholder Rendering Args (Device, Key, Database, etc...)</param>
        public new void Process(GetPlaceholderRenderingsArgs args)
        {
            Assert.IsNotNull((object)args, "args");

            //Remove the dynamic indexing from the key
            var key = string.Join("/", args.PlaceholderKey.Split('/').Select(i => i.Split('#')[0]));

            //Get the placeholder item, if there is a Device selected get it using that Device Context
            Item placeholderItem = null;
            if (ID.IsNullOrEmpty(args.DeviceId))
            {
                placeholderItem = Client.Page.GetPlaceholderItem(key, args.ContentDatabase, args.LayoutDefinition);
            }
            else
            {
                using (new DeviceSwitcher(args.DeviceId, args.ContentDatabase))
                {
                    placeholderItem = Client.Page.GetPlaceholderItem(key, args.ContentDatabase, args.LayoutDefinition);
                }
            }
            
            //If there was a placeholder item (Placeholder Settings) set the allowed renderings
            if (placeholderItem != null)
            {
                args.HasPlaceholderSettings = true;
                bool allowedControlsSpecified;
                args.PlaceholderRenderings = this.GetRenderings(placeholderItem, out allowedControlsSpecified);
                if (allowedControlsSpecified)
                    args.Options.ShowTree = false;
            }
        }
    }
}
