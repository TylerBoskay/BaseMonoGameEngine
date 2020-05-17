using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace BaseMonoGameEngine
{
    /// <summary>
    /// A derived <see cref="ContentManager"/> that provides extra functionality.
    /// </summary>
    public sealed class BContentManager : ContentManager
    {
        public BContentManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public BContentManager(IServiceProvider serviceProvider, string rootDirectory) : base(serviceProvider, rootDirectory)
        {

        }

        /// <summary>
        /// Tells how many assets are currently loaded.
        /// </summary>
        /// <returns>An integer representing how many assets are loaded.</returns>
        public int LoadedAssetCount => LoadedAssets.Count;

        /// <summary>
        /// Tells if an asset with a particular name is loaded.
        /// </summary>
        /// <param name="assetName">The name of the asset.</param>
        /// <returns>true if the asset is loaded, otherwise false.</returns>
        public bool IsAssetLoaded(in string assetName)
        {
            return LoadedAssets.ContainsKey(assetName);
        }
    }
}