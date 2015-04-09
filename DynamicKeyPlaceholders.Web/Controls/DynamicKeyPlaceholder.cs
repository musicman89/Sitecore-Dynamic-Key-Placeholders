using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Sitecore.Common;
using Sitecore.Layouts;
using Sitecore.Web.UI;
using Sitecore.Web.UI.WebControls;

namespace DragoniaStudios.SC.UI.Controls
{
    public class DynamicKeyPlaceholder : WebControl, IExpandable
    {
        protected string _key = Placeholder.DefaultPlaceholderKey;
        protected string _dynamicKey = null;
        protected Placeholder _placeholder;

        /// <summary>
        ///
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value.ToLower(); }
        }

        protected string DynamicKey
        {
            get
            {
                if (_dynamicKey != null)
                {
                    return _dynamicKey;
                }
                _dynamicKey = _key;

                // Find the last placeholder processed.
                Stack<Placeholder> stack = Switcher<Placeholder,
                    PlaceholderSwitcher>.GetStack(false);
                Placeholder current = stack.Peek();

                // Group of containers which sit in the current placeholder (i.e. they have the same placeholder id).
                var renderings = Sitecore.Context.Page.Renderings.Where(rendering => (rendering.Placeholder == current.ContextKey || rendering.Placeholder == current.Key) && rendering.AddedToPage).ToArray();

                // Current container
                var thisRendering = renderings.Last();

                // get all repeating containers in the current placeholder
                renderings = renderings.Where(i => i.RenderingItem != null && i.RenderingItem.ID == thisRendering.RenderingItem.ID).ToArray();

                // Count represents how many of the same container have already been added to the page
                if (renderings.Any())
                {
                    _dynamicKey = _key + "#" + renderings.Count();
                }
                return _dynamicKey;
            }
        }

        protected override void CreateChildControls()
        {
            _placeholder = new Placeholder { Key = DynamicKey };
            Controls.Add(_placeholder);
            _placeholder.Expand();
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            base.RenderChildren(output);
        }

        #region IExpandable Members

        /// <summary>
        ///
        /// </summary>
        public void Expand()
        {
            EnsureChildControls();
        }

        #endregion
    }
}
