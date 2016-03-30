using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AtelierXNA
{
   public class Sprite : Microsoft.Xna.Framework.DrawableGameComponent
   {
      string NomImage { get; set; }
      float �chelle { get; set; }
      Vector2 Origine { get; set; }
      protected Vector2 Position { get; set; }        // En pr�vision d'une sp�cialisation vers un sprite dynamique
      protected Texture2D Image { get; private set; } // En pr�vision d'une sp�cialisation vers un sprite anim�
      protected SpriteBatch GestionSprites { get; private set; } // En pr�vision d'une sp�cialisation vers un sprite anim�
      RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

      public Sprite(Game jeu, string nomImage, Vector2 position,float �chelle)
         : base(jeu)
      {
         NomImage = nomImage;
         Position = position;
         �chelle = �chelle;
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
         GestionSprites.Draw(Image, Position, null,Color.White,0,Origine,�chelle,SpriteEffects.None,0f);         
         base.Draw(gameTime);
         GestionSprites.End();
      }
   }
}