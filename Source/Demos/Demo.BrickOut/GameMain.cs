﻿using System.Reflection;
using Demo.BrickOut.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Components;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace Demo.BrickOut
{
    public class GameMain : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphicsDeviceManager;
        private ViewportAdapter _viewportAdapter;

        public const int VirtualWidth = 1024;
        public const int VirtualHeight = 768;

        private readonly EntityComponentSystem _entityComponentSystem;
        private readonly EntityManager _entityManager;

        public GameMain()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = VirtualWidth,
                PreferredBackBufferHeight = VirtualHeight
            };
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _entityComponentSystem = new EntityComponentSystem(this);
            _entityManager = _entityComponentSystem.EntityManager;

            // scan for components and systems in provided assemblies
            _entityComponentSystem.Scan(Assembly.GetExecutingAssembly());
        }

        protected override void LoadContent()
        {
            Services.AddService(new SpriteBatch(GraphicsDevice));

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, VirtualWidth, VirtualHeight);
            var camera = new Camera2D(_viewportAdapter);
            Services.AddService(camera);

            var textureAtlas = Content.Load<TextureAtlas>("brick-out-atlas");
            Services.AddService<ITextureRegionService>(new TextureRegionService(textureAtlas));

            _entityComponentSystem.Initialize();

            _entityManager.CreateEntityFromTemplate(nameof(Ball));
            _entityManager.CreateEntityFromTemplate(nameof(Paddle));
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            _entityComponentSystem.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _entityComponentSystem.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
