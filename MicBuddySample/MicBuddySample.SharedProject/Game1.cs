using System;
using System.Threading;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Diagnostics;
using FontBuddyLib;
using MicBuddyLib;
using System.Collections.Generic;
using GameTimer;
using Plugin.Permissions;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;

namespace MicBuddySample
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{
		#region Fields

		GraphicsDeviceManager graphics;

		SpriteBatch spriteBatch;
		GameClock _clock = new GameClock();
		FontBuddy font = new FontBuddy();

		IMicrophoneComponent _mics;
		List<IMicrophoneHelper> _helpers = new List<IMicrophoneHelper>();

		#endregion

		#region Methods

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = false;

			_mics = new MicrophoneComponent(this);
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

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			//TODO: use this.Content to load your game content here 

			Task.Run(() => GetMicPermission());

			font.Font = Content.Load<SpriteFont>("ArialBlack24");

			foreach (var mic in _mics.AvailableMicrophones)
			{
				_helpers.Add(new MicrophoneHelper(_mics, mic));
			}

			_clock.Start();
		}

		protected async Task<bool> GetMicPermission()
		{
//#if __IOS || ANDROID
			try
			{
				var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Microphone);
				if (status != PermissionStatus.Granted)
				{
					var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Microphone);
					status = results[Permission.Location];
				}

				return (status == PermissionStatus.Granted);
			}
			catch (Exception)
			{
			}

			return false;
			//#endif
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			foreach (var helper in _helpers)
			{
				helper.StopRecording();
			}
			base.OnExiting(sender, args);
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
#if !__IOS__
				this.Exit();
#endif
			}

			// TODO: Add your update logic here	
			base.Update(gameTime);

			_clock.Update(gameTime);
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
			foreach (var helper in _helpers)
			{
				Color drawColor = helper.IsTalking ? Color.Green : Color.Red;
				//write the mic name
				font.Write(helper.MicrophoneName,
				           pos,
				           Justify.Left,
							1f,
				           drawColor,
				           spriteBatch,
						   _clock);

				pos.Y += font.Font.LineSpacing * 0.5f;

				//Draw the current max volume
				font.Write("avg volume: " + helper.AverageVolume.ToString("F3"),
				           pos,
				           Justify.Left,
				           1f,
				           drawColor,
				           spriteBatch,
						   _clock);

				pos.Y += font.Font.LineSpacing * 0.5f;

				//Draw the current volume
				font.Write("cur volume: " + helper.CurrentVolume.ToString("F3"),
				           pos,
				           Justify.Left,
				           1f,
				           drawColor,
				           spriteBatch,
						   _clock);

				pos.Y += font.Font.LineSpacing;
			}
			
			spriteBatch.End();
            
			base.Draw(gameTime);
		}

#endregion //Methods
	}
}
