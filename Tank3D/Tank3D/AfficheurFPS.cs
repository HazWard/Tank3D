using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AtelierXNA
{
   public class AfficheurFPS : Microsoft.Xna.Framework.DrawableGameComponent
   {
      SpriteBatch GestionSprites { get; set; }
      CalculateurFPS GestionFPS { get; set; }
      Vector2 PositionDroiteBas { get; set; }
      Vector2 PositionChaîne { get; set; }
      string ChaîneFPS { get; set; }
      SpriteFont ArialFont { get; set; }

      public AfficheurFPS(Game game)
         : base(game)
      {
      }

      public override void Initialize()
      {
          const int MARGE_BAS = 10;
          const int MARGE_DROITE = 15;

          PositionDroiteBas = new Vector2(Game.Window.ClientBounds.Width - MARGE_DROITE,
                                         Game.Window.ClientBounds.Height - MARGE_BAS);
         ChaîneFPS = "";
         base.Initialize();
      }

      protected override void LoadContent()
      {
         GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
         GestionFPS = Game.Services.GetService(typeof(CalculateurFPS)) as CalculateurFPS;
         ArialFont = Game.Content.Load<SpriteFont>("Fonts/Arial");
         base.LoadContent();
      }

      public override void Update(GameTime gameTime)
      {
         if (GestionFPS.ChaîneFPS != ChaîneFPS)
         {
            ChaîneFPS = GestionFPS.ChaîneFPS;
            Vector2 dimension = ArialFont.MeasureString(ChaîneFPS);
            PositionChaîne = PositionDroiteBas - dimension;
         }
         base.Update(gameTime);
      }

      public override void Draw(GameTime gameTime)
      {
         GestionSprites.DrawString(ArialFont, ChaîneFPS, PositionChaîne, Color.Tomato, 0,
                                   Vector2.Zero, 1f, SpriteEffects.None, 0);
         base.Draw(gameTime);
      }
   }
}