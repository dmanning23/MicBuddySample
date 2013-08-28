using System;
using System.Threading;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using FontBuddyLib;
using MicBuddy;
using System.Collections.Generic;

namespace MicTest
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		#region Audio Out

		#endregion

		#region Audio In

		//Create the audio in component
		List<Microphone> microphones = new List<Microphone>();

		FontBuddy font = new FontBuddy();

		#endregion

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = true;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			base.Initialize();
				
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			foreach (Microphone mic in microphones)
			{
				mic.StopRecording();
			}
			base.OnExiting(sender, args);
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			//TODO: use this.Content to load your game content here 

			font.Font = Content.Load<SpriteFont>("ArialBlack24");

			//Initialize the Microphone using the default microphone
			Microphone.EnumerateMicrophones();
			foreach (string mic in Microphone.AvailableMicrophones)
			{
				Microphone dude = new Microphone(mic);
				dude.MicSensitivity = 0.1f;
				dude.StartRecording();
				microphones.Add(dude);
			}
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) || 
			    Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				this.Exit();
			}

			// TODO: Add your update logic here	
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();

			//write all the available microphones
			Vector2 pos = new Vector2(0.0f, 0.0f);
			foreach (Microphone mic in microphones)
			{
				Color drawColor = mic.IsTalking ? Color.Green : Color.Red;
				//write the mic name
				font.Write(mic.MicrophoneName,
				           pos,
				           Justify.Left,
							0.5f,
				           drawColor,
				           spriteBatch,
				           0.0f);

				pos.Y += font.Font.LineSpacing * 0.5f;

				//Draw the current max volume
				font.Write("avg volume: " + mic.AverageVolume.ToString("F3"),
				           pos,
				           Justify.Left,
				           0.5f,
				           drawColor,
				           spriteBatch,
				           0.0f);

				pos.Y += font.Font.LineSpacing * 0.5f;

				//Draw the current volume
				font.Write("cur volume: " + mic.CurrentVolume.ToString("F3"),
				           pos,
				           Justify.Left,
				           0.5f,
				           drawColor,
				           spriteBatch,
				           0.0f);

				pos.Y += font.Font.LineSpacing;
			}
			
			spriteBatch.End();
            
			base.Draw(gameTime);
		}
	}
}

