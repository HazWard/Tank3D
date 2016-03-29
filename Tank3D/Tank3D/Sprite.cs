using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AtelierXNA
{
   public class Sprite : Microsoft.Xna.Framework.DrawableGameComponent
   {
      string NomImage { get; set; }
      float Échelle { get; set; }
      Vector2 Origine { get; set; }
      protected Vector2 Position { get; set; }        // En prévision d'une spécialisation vers un sprite dynamique
      protected Texture2D Image { get; private set; } // En prévision d'une spécialisation vers un sprite animé
      protected SpriteBatch GestionSprites { get; private set; } // En prévision d'une spécialisation vers un sprite animé
      RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

      public Sprite(Game jeu, string nomImage, Vector2 position,float échelle)
         : base(jeu)
      {
         NomImage = nomImage;
         Position = position;
         Échelle = échelle;
      }

      protected override void LoadContent()
      {
         GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
         GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
         Image = GestionnaireDeTextures.Find(NomImage);
         Origine = new Vector2((Image.Width / 2) -30, Image.Height *2);
         //Position = new Vector2(Position.X - Orig, Position.Y - (Image.Height/2));
      }

      public override void Draw(GameTime gameTime)
      {
         GestionSprites.Begin();
         GestionSprites.Draw(Image, Position, null,Color.White,0,Origine,Échelle,SpriteEffects.None,0f);         
         base.Draw(gameTime);
         GestionSprites.End();
      }
   }
}