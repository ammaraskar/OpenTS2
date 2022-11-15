﻿using OpenTS2.Assemblies;
using OpenTS2.Client;
using OpenTS2.Content;
using OpenTS2.Files;
using OpenTS2.Files.Formats.DBPF;
using System.Reflection;
using UnityEngine;

namespace OpenTS2.Engine.Core
{
    /// <summary>
    /// Main initialization class for Unity Engine implementation of OpenTS2.
    /// </summary>
    public static class Main
    {
        static bool s_initialized = false;
        /// <summary>
        /// Initializes all singletons, systems and the game assembly.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (s_initialized)
                return;
            var settings = new Settings();
            var epManager = new EPManager();
            var contentManager = new ContentManager();
            var objectManager = new ObjectManager(contentManager.Provider);
            Filesystem.Initialize(new JSONPathProvider(), epManager);
            Factories.TextureFactory = new TextureFactory();
            CodecAttribute.Initialize();
            //Initialize the game assembly, do all reflection things.
            AssemblyHelper.InitializeAssembly(Assembly.GetExecutingAssembly());
            s_initialized = true;
        }
    }
}