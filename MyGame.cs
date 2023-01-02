﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RocketJumper.Classes;
using RocketJumper.Classes.States;
using Newtonsoft.Json.Linq;
using System.IO;

namespace RocketJumper
{
    public class MyGame : Game
    {
        private State currentState;
        private State nextState;

        // content
        public SpriteFont Font;


        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static int VirtualWidth;
        public static int VirtualHeight;
        public static int ActualWidth;
        public static int ActualHeight;

        public MyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            // screen properties
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.ApplyChanges();

            ActualWidth = graphics.PreferredBackBufferWidth;
            ActualHeight = graphics.PreferredBackBufferHeight;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Font = Content.Load<SpriteFont>("Fonts/TimerFont");

            currentState = new MenuState(this, Content);
            currentState.LoadContent();
            nextState = null;
        }

        protected override void Update(GameTime gameTime)
        {
            if (nextState != null)
            {
                currentState = nextState;
                if (!nextState.IsPaused)
                    currentState.LoadContent();
                nextState = null;
            }

            currentState.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            currentState.Draw(gameTime, spriteBatch);
            base.Draw(gameTime);
        }

        public void ChangeState(State state)
        {
            nextState = state;
        }

        public Texture2D GetFrame()
        {
            Texture2D texture = new Texture2D(GraphicsDevice, ActualWidth, ActualHeight);
            Color[] data = new Color[ActualWidth * ActualHeight];
            GraphicsDevice.GetBackBufferData(data);
            texture.SetData(data);
            return texture;
        }
    }
}