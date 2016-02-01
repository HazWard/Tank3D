using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
   class TextureInvalideException : ApplicationException { }
   public class TexturesManager
   {
      Game Jeu { get; set; }
      string RépertoireDesTextures { get; set; }
      List<TextureDeBase> ListeTextures { get; set; }

      public TexturesManager(Game jeu, string répertoireDesTextures)
      {
         Jeu = jeu;
         RépertoireDesTextures = répertoireDesTextures;
         ListeTextures = new List<TextureDeBase>();
      }

      public void Add(string nom, Texture2D texture2DÀAjouter)
      {
         TextureDeBase textureÀAjouter = new TextureDeBase(nom, texture2DÀAjouter);
         if (!ListeTextures.Contains(textureÀAjouter))
         {
            ListeTextures.Add(textureÀAjouter);
         }
         else
         {
             throw new TextureInvalideException();
         }
      }

      void Add(TextureDeBase textureÀAjouter)
      {
         textureÀAjouter.Load();
         ListeTextures.Add(textureÀAjouter);
      }

      public Texture2D Find(string nomTexture)
      {
         const int TEXTURE_PAS_TROUVÉE = -1;
         TextureDeBase textureÀRechercher = new TextureDeBase(Jeu.Content, RépertoireDesTextures, nomTexture);
         int indexTexture = ListeTextures.IndexOf(textureÀRechercher);
         if (indexTexture == TEXTURE_PAS_TROUVÉE)
         {
            Add(textureÀRechercher);
            indexTexture = ListeTextures.Count - 1;
         }
         return ListeTextures[indexTexture].Image;
      }
   }
}
