using System;
using System.Collections;

using System.Diagnostics;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;
using Sce.PlayStation.Core.Imaging;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.UI;
using Sce.PlayStation.Core.Audio;

namespace Dodge
{
	public class InGameScene : GameScene
	{
		private Tank tank;
		private Spawner tankSpawner;
		private Tank[] tankList;
		private Player player;
		private Sce.PlayStation.HighLevel.UI.Scene uiScene;
		private Sce.PlayStation.HighLevel.UI.Label uiScore;
		private Sce.PlayStation.HighLevel.UI.Label uiTime;
		private bool alive;
		private int numTanks;
		private BgmPlayer bgSoundPlayer;
    	private Bgm bgSound;
			
		public InGameScene ()
		{
			
			InitializeUI();
			
			scene = new Sce.PlayStation.HighLevel.GameEngine2D.Scene();
			scene.Camera.SetViewFromViewport();
			
			textureInfo = new TextureInfo("/Application/Assets/stone2.png");
			background = new SpriteUV(textureInfo);
			background.Quad.S = textureInfo.TextureSizef;
			scene.AddChild(background);
			numTanks = 5;
			//tank = new Tank(scene);
			//tankSpawner = new Spawner(tank);
			tankList = new Tank[numTanks];
			for (int i = 0; i < numTanks; i++) 
			{
				tankList[i] = new Tank(scene);
			}
			
			player = new Player(scene);
			
			ScoreManager.Instance.startTime();
			alive = true;
			
			bgSound = new Bgm("/Application/Audio/BenHill.mp3");
			bgSoundPlayer = bgSound.CreatePlayer();
			//Fix when it plays
			
			//bgSoundPlayer.Dispose();
			
			
			
			
		}
		private void InitializeUI()
		{

			uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
			Panel panel = new Panel();
			panel.Width = Director.Instance.GL.Context.GetViewport().Width;
			panel.Height = Director.Instance.GL.Context.GetViewport().Height;
			
			uiScore = new Sce.PlayStation.HighLevel.UI.Label();
			uiScore.Text = "Score: ";
			uiScore.HorizontalAlignment = HorizontalAlignment.Right;
			uiScore.VerticalAlignment = VerticalAlignment.Top;
			uiScore.SetPosition(Director.Instance.GL.Context.GetViewport().Width - uiScore.Width, Director.Instance.GL.Context.GetViewport().Height*.01f);		
			uiScore.TextColor = new UIColor(1.0f, 0.0f, 0.0f, 1.0f);
			uiScore.Font = new UIFont(FontAlias.System, 25, FontStyle.Bold);
			
			uiTime = new Sce.PlayStation.HighLevel.UI.Label();
			uiTime.Text = "Time: ";
			uiTime.HorizontalAlignment = HorizontalAlignment.Left;
			uiTime.VerticalAlignment = VerticalAlignment.Top;
			uiTime.SetPosition(0.0f, Director.Instance.GL.Context.GetViewport().Height*.01f);		
			uiTime.TextColor = new UIColor(1.0f, 1.0f, 0.0f, 1.0f);
			uiTime.Font = new UIFont(FontAlias.System, 25, FontStyle.Bold);
			
			panel.AddChildLast(uiScore);
			panel.AddChildLast(uiTime);
			uiScene.RootWidget.AddChildLast(panel);
			UISystem.SetScene(uiScene);
		}
		
		public override void Update(float dT)
		{
			uiTime.Text = "Time: " + ScoreManager.Instance.getTime();
			uiScore.Text = "Score: " + ScoreManager.Instance.getScore();

			foreach(Tank tank in tankList)
			{
				tank.Update(dT);
				tank.rotateTurret(player.Position(), dT);
				if(player.isTouching() && player.Collision(tank.getPosition(), tank.getSize()))
				   alive = false;

			}
			
			player.Update();
			if(player.isTouching())
			{
				ScoreManager.Instance.runScore();
				if(bgSoundPlayer.Status == BgmStatus.Paused)
					bgSoundPlayer.Resume();
				else if(bgSoundPlayer.Status != BgmStatus.Playing)
					bgSoundPlayer.Play();
				
			}
			else 
			{
				
				bgSoundPlayer.Pause();
			}
			
			
			if(!alive)
			{
				bgSoundPlayer.Dispose();
				ScoreManager.Instance.setScore();
				SceneManager.Instance.setEndScene();
			}
			
			Director.Instance.Update();
			
		}
		public override void Draw(float dT)
		{
			
			Director.Instance.Render();
			UISystem.Render();
			Director.Instance.GL.Context.SwapBuffers();
			Director.Instance.PostSwap();
		}
		public void addEnemy()
		{
			//tankList.Add(tankSpawner.SpawnEnemy());
		}
	}
}

